namespace CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;

public interface IEtherscanApiKeyProvider
{
    string ApiKey();
}