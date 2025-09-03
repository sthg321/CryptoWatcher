using Ardalis.Specification;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.UniswapModule.Entities;

namespace CryptoWatcher.UniswapModule.Specifications;

/// <summary>
/// Specification to retrieve active liquidity pool positions filtered by
/// the provided Uniswap network and wallet.
/// </summary>
/// <remarks>
/// This specification applies the following criteria:
/// 1. Filters on the network name of the Uniswap network.
/// 2. Filters on the wallet address.
/// 3. Ensures positions are active.
/// Additionally, it includes the associated pool position snapshots for the queried results.
/// </remarks>
/// <param name="uniswapNetwork">The Uniswap network to filter positions by.</param>
/// <param name="wallet">The wallet to filter positions by.</param>
internal sealed class GetPositionsByWalletAndNetworkSpecification : Specification<PoolPosition>
{
    public GetPositionsByWalletAndNetworkSpecification(UniswapNetwork uniswapNetwork, Wallet wallet)
    {
        Query
            .Include(position => position.PoolPositionSnapshots)
            .Where(position => position.UniswapNetwork.Name == uniswapNetwork.Name &&
                               position.Wallet.Address == wallet.Address &&
                               position.IsActive);
    }
}