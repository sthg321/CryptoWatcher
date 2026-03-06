namespace CryptoWatcher.Modules.WalletIngestion.Infrastructure.Integrations.Configs;

public class KafkaConfig
{
    public Uri Host { get; set; } = null!;

    public string RawTransactionsTopic { get; set; } = null!;
}