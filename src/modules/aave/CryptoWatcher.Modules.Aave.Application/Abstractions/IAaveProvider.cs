using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Aave.Models;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Aave.Application.Abstractions;

/// <summary>
/// Defines methods for interacting with the Aave protocol, allowing retrieval of lending positions and other related data.
/// </summary>
public interface IAaveProvider
{
    /// <summary>
    /// Retrieves the list of lending positions from the Aave protocol for a specified network and wallet.
    /// </summary>
    /// <param name="protocol"></param>
    /// <param name="wallet">The wallet whose lending positions will be queried.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of <see cref="AaveLendingPosition"/> objects.</returns>
    Task<AavePositionsResponse> GetLendingPositionAsync(AaveProtocolConfiguration protocol, Wallet wallet);
}