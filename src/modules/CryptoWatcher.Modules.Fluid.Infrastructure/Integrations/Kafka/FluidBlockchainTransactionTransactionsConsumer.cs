using System.Text.Json;
using Confluent.Kafka;
using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.Fluid.Application.Features.Abstractions;
using CryptoWatcher.Modules.Infrastructure.Shared.Configs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nethereum.JsonRpc.Client;
using Polly;
using Polly.Wrap;

namespace CryptoWatcher.Modules.Fluid.Infrastructure.Integrations.Kafka;

public class FluidBlockchainTransactionTransactionsConsumer : BackgroundService
{
    private const int MaxRetries = 3;
    private const int BatchSize = 50;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly KafkaConfig _config;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<FluidBlockchainTransactionTransactionsConsumer> _logger;
    
    private readonly AsyncPolicyWrap _retryPolicy;
    
    public FluidBlockchainTransactionTransactionsConsumer(KafkaConfig config, IServiceScopeFactory scopeFactory,
        ILogger<FluidBlockchainTransactionTransactionsConsumer> logger)
    {
        _config = config;
        _scopeFactory = scopeFactory;
        _logger = logger;
        
        var retry = Policy
            .Handle<Exception>(IsTransient)
            .WaitAndRetryAsync(MaxRetries, i => TimeSpan.FromSeconds(Math.Pow(2, i)));

        var circuitBreaker = Policy
            .Handle<Exception>(IsTransient)
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30));

        _retryPolicy = Policy.WrapAsync(retry, circuitBreaker);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<string, string>(new ConsumerConfig
        {
            GroupId = "fluid",
            EnableAutoCommit = false,
            EnableAutoOffsetStore = false,
            BootstrapServers = _config.Host.ToString(),
            AutoOffsetReset = AutoOffsetReset.Earliest
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
                var consumerService = scope.ServiceProvider.GetRequiredService<IFluidTransactionConsumer>();

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
            }
        }
    }

    private static IEnumerable<ConsumeResult<string, string>> ConsumeBatch(
        IConsumer<string, string> consumer)
    {
        for (var i = 0; i < BatchSize; i++)
        {
            var result = consumer.Consume(TimeSpan.FromSeconds(5));
            if (result is null)
            {
                yield break;
            }

            yield return result;
        }
    }

    private BlockchainTransaction? DeserializeTransaction(ConsumeResult<string, string> result)
    {
        try
        {
            return JsonSerializer.Deserialize<BlockchainTransaction>(result.Message.Value, JsonSerializerOptions);
        }
        catch (JsonException e)
        {
            _logger.LogError(e,
                "Failed to deserialize message at {Topic}/{Partition}:{Offset}",
                result.Topic, result.Partition.Value, result.Offset.Value);
            return null;
        }
    }

    private async Task ProcessWithRetryAsync(
        IFluidTransactionConsumer consumerService,
        BlockchainTransaction transaction,
        CancellationToken stoppingToken)
    {
        using var _ = _logger.BeginScope(
            "Transaction hash: {TransactionHash}. ChainId: {ChainId}",
            transaction.Hash,
            transaction.ChainId);

        await _retryPolicy.ExecuteAsync(async ct =>
        {
            await consumerService.ConsumeAsync(transaction, ct);
        }, stoppingToken);
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
