using CryptoWatcher.AaveModule.Models;

namespace CryptoWatcher.AaveModule.Abstractions;

public interface IAaveMainnetProvider
{
    string GetMainnetAddressByNetworkName(AaveNetwork aaveNetwork);
}