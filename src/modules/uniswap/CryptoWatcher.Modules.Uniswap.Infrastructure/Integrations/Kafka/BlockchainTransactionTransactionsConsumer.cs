using System.Text.Json;
using Confluent.Kafka;
using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.WalletIngestion.Infrastructure.Integrations.Configs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nethereum.JsonRpc.Client;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Kafka;

public class BlockchainTransactionTransactionsConsumer : BackgroundService
{
    private const int MaxRetries = 3;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly KafkaConfig _config;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BlockchainTransactionTransactionsConsumer> _logger;

    public BlockchainTransactionTransactionsConsumer(KafkaConfig config, IServiceScopeFactory scopeFactory,
        ILogger<BlockchainTransactionTransactionsConsumer> logger)
    {
        _config = config;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<string, string>(new ConsumerConfig
        {
            GroupId = "uniswap",
            EnableAutoCommit = false,
            BootstrapServers = _config.Host.ToString()
        }).Build();

        consumer.Subscribe(_config.RawTransactionsTopic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var batch = ConsumeBatch(consumer, 50);

                if (batch.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                    continue;
                }

                await ProcessBatchSequentially(batch, stoppingToken);

                consumer.Commit(batch.Last());
            }
            catch (ConsumeException e)
            {
                _logger.LogError(e, "Kafka consume error");
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    private async Task ProcessBatchSequentially(
        List<ConsumeResult<string, string>> batch,
        CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var consumerService = scope.ServiceProvider.GetRequiredService<IWalletTransactionConsumer>();

        foreach (var message in batch)
        {
            if (message.Message.Value is null)
            {
                continue;
            }

            BlockchainTransaction transaction;
            try
            {
                transaction = JsonSerializer.Deserialize<BlockchainTransaction>(
                    message.Message.Value, JsonSerializerOptions)!;
            }
            catch (JsonException e)
            {
                _logger.LogError(e,
                    "Failed to deserialize message at {Topic}/{Partition}:{Offset}, skipping",
                    message.Topic, message.Partition.Value, message.Offset.Value);
                continue;
            }

            await ProcessWithRetryAsync(consumerService, transaction, stoppingToken);
        }
    }

    private async Task ProcessWithRetryAsync(
        IWalletTransactionConsumer consumerService,
        BlockchainTransaction transaction,
        CancellationToken stoppingToken)
    {
        for (var attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                await consumerService.ConsumeTransactionAsync(transaction, stoppingToken);
                return;
            }
            catch (Exception e) when (IsTransient(e) && attempt < MaxRetries)
            {
                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                _logger.LogWarning(e,
                    "Transient error processing transaction {Hash}, attempt {Attempt}/{MaxRetries}. Retrying in {Delay}s",
                    transaction.Hash, attempt, MaxRetries, delay.TotalSeconds);
                await Task.Delay(delay, stoppingToken);
            }
            catch (Exception e) when (IsTransient(e))
            {
                _logger.LogError(e,
                    "Transaction {Hash} failed after {MaxRetries} attempts due to transient error. Stopping batch to preserve ordering",
                    transaction.Hash, MaxRetries);
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Permanent error processing transaction {Hash}. Stopping batch to preserve ordering",
                    transaction.Hash);
                throw;
            }
        }
    }

    private static bool IsTransient(Exception exception)
    {
        return exception is HttpRequestException
            or RpcResponseException
            or RpcClientTimeoutException
            or RpcClientUnknownException
            or TimeoutException
            or TaskCanceledException { InnerException: TimeoutException };
    }

    private static List<ConsumeResult<string, string>> ConsumeBatch(
        IConsumer<string, string> consumer,
        int batchSize)
    {
        var batch = new List<ConsumeResult<string, string>>(batchSize);

        for (var i = 0; i < batchSize; i++)
        {
            var result = consumer.Consume(TimeSpan.FromMilliseconds(100));

            if (result is null)
            {
                break;
            }

            batch.Add(result);
        }

        return batch;
    }
}