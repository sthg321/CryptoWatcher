using CryptoWatcher.Modules.Aave.Models;

namespace CryptoWatcher.Modules.Aave.Abstractions;

/// <summary>
/// Defines an interface for retrieving mainnet addresses associated with specific Aave networks.
/// This is typically used to map Aave network configurations to their respective mainnet addresses.
/// </summary>
public interface IAaveMainnetProvider
{
    /// <summary>
    /// Retrieves the mainnet address associated with a specific Aave network.
    /// </summary>
    /// <param name="aaveNetwork">An instance of <see cref="AaveNetwork"/> representing the target Aave network.</param>
    /// <returns>The mainnet address as a string corresponding to the specified Aave network.</returns>
    string GetMainnetAddressByNetworkName(AaveNetwork aaveNetwork);
}