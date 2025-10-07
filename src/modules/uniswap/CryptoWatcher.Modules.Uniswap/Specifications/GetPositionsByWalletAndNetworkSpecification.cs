using Ardalis.Specification;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Uniswap.Specifications;

public sealed class GetPositionsByWalletAndNetworkSpecification : Specification<PoolPosition>
{
    public GetPositionsByWalletAndNetworkSpecification(UniswapChainConfiguration uniswapNetwork, Wallet wallet)
    {
        Query
            .Include(position => position.PoolPositionSnapshots)
            .Where(position => position.NetworkName == uniswapNetwork.Name &&
                               position.ProtocolVersion == uniswapNetwork.ProtocolVersion &&
                               position.Wallet.Address == wallet.Address &&
                               position.IsActive);
    }
}