using CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace CryptoWatcher.Modules.WalletIngestion.Infrastructure.Integrations.Etherscan;

public class EtherscanApiKeyProvider : IEtherscanApiKeyProvider
{
    private readonly IConfiguration _configuration;

    public EtherscanApiKeyProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string ApiKey()
    {
        return _configuration.GetValue<string>("EtherscanApiKey")!;
    }
}