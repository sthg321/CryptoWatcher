namespace CryptoWatcher.Modules.Infrastructure.Shared.Configs;

public class KafkaConfig
{
    public Uri Host { get; set; } = null!;

    public string RawTransactionsTopic { get; set; } = null!;

    public string UniswapConsumerGroupId { get; set; } = null!;
}