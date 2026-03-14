using System.Text.Json;
using Confluent.Kafka;
using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.Infrastructure.Shared.Configs;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nethereum.JsonRpc.Client;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Kafka;

public class BlockchainTransactionTransactionsConsumer : BackgroundService
{
    private const int MaxRetries = 3;
    private const int BatchSize = 50;
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
            GroupId = _config.UniswapConsumerGroupId,
            EnableAutoCommit = false,
            EnableAutoOffsetStore = false,
            BootstrapServers = _config.Host.ToString()
        }).Build();

        consumer.Subscribe(_config.RawTransactionsTopic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var batch = ConsumeBatch(consumer).ToArray();
                if (batch.Length == 0)
                {
                    continue;
                }

                using var scope = _scopeFactory.CreateScope();
                var consumerService = scope.ServiceProvider.GetRequiredService<IWalletTransactionConsumer>();

                foreach (var result in batch)
                {
                    var transaction = DeserializeTransaction(result);

                    if (transaction is not null)
                    {
                        await ProcessWithRetryAsync(consumerService, transaction, stoppingToken);
                    }

                    consumer.StoreOffset(result);
                }

                consumer.Commit();
            }
            catch (ConsumeException e)
            {
                _logger.LogError(e, "Kafka consume error");
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing batch. Committing last stored offset");
                consumer.Commit();
            }
        }
    }

    private static IEnumerable<ConsumeResult<string, string>> ConsumeBatch(
        IConsumer<string, string> consumer)
    {
        for (var i = 0; i < BatchSize; i++)
        {
            var result = consumer.Consume(TimeSpan.FromMilliseconds(100));
            if (result is null)
            {
                yield break;
            }

            yield return result;
        }
    }

    private BlockchainTransaction? DeserializeTransaction(ConsumeResult<string, string> result)
    {
        if (result.Message.Value is null)
            return null;

        try
        {
            return JsonSerializer.Deserialize<BlockchainTransaction>(result.Message.Value, JsonSerializerOptions);
        }
        catch (JsonException e)
        {
            _logger.LogError(e,
                "Failed to deserialize message at {Topic}/{Partition}:{Offset}",
                result.Topic, result.Partition.Value, result.Offset.Value);
            throw;
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
                using var _ = _logger.BeginScope("Transaction hash: {TransactionHash}. ChainId: {ChainId}",
                    transaction.Hash, transaction.ChainId);
                
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
}
