using System.Text.Json;
using Confluent.Kafka;
using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.Infrastructure.Shared.Configs;
using CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;
using CryptoWatcher.Modules.WalletIngestion.Application.Models;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.WalletIngestion.Infrastructure.Integrations.Kafka;

public class WalletTransactionProducer : IWalletTransactionProducer, IDisposable
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private const string KeyTemplate = "{0}:{1}";

    private readonly IProducer<string, string> _producer;
    private readonly string _topic;
    private readonly ILogger<WalletTransactionProducer> _logger;

    public WalletTransactionProducer(KafkaConfig kafkaConfig, ILogger<WalletTransactionProducer> logger)
    {
        _logger = logger;
        _producer = new ProducerBuilder<string, string>(new ProducerConfig
        {
            BootstrapServers = kafkaConfig.Host.ToString(),
            EnableIdempotence = true,
            Acks = Acks.All,
            CompressionType = CompressionType.Lz4,
            LingerMs = 5
        }).Build();

        _topic = kafkaConfig.RawTransactionsTopic;
    }

    public async Task ProduceAsync(BlockchainTransaction transaction, CancellationToken ct = default)
    {
        var message = new Message<string, string>
        {
            Key = string.Format(KeyTemplate, transaction.ChainId, transaction.From.Value),
            Value = JsonSerializer.Serialize(transaction, JsonSerializerOptions),
            Timestamp = new Timestamp(transaction.Timestamp)
        };

        try
        {
            await _producer.ProduceAsync(_topic, message, ct);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while producing transaction");
        }
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
        GC.SuppressFinalize(this);
    }
}