using AaveClient;
using CryptoWatcher.Modules.Aave.Abstractions;
using CryptoWatcher.Modules.Aave.Models;
using CryptoWatcher.Infrastructure.Configs;
using CryptoWatcher.Infrastructure.Extensions;

namespace CryptoWatcher.Infrastructure.Aave;

internal class AaveMainnetProvider : IAaveMainnetProvider
{
    private readonly AaveConfig _aaveConfig;

    public AaveMainnetProvider(AaveConfig aaveConfig)
    {
        _aaveConfig = aaveConfig;
    }

    public string GetMainnetAddressByNetworkName(AaveNetwork aaveNetwork)
    {
        if (!Enum.TryParse<AaveNetworkType>(aaveNetwork.Name, true, out var network))
        {
            throw new ArgumentException(
                $"Network {aaveNetwork.Name} is not supported. Supported networks: {string.Join(", ", Enum.GetNames<AaveNetworkType>())}");
        }

        return network.GetMainnetAddress(_aaveConfig);
    }
}