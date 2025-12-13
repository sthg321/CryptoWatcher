using Ardalis.Specification;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Uniswap.Specifications;

public sealed class GetPositionsByWalletAndNetworkSpecification : Specification<UniswapLiquidityPosition>
{
    public GetPositionsByWalletAndNetworkSpecification(UniswapChainConfiguration uniswapNetwork, Wallet wallet)
    {
        Query
            .Include(position => position.PositionSnapshots)
            .Where(position => position.NetworkName == uniswapNetwork.Name &&
                               position.ProtocolVersion == uniswapNetwork.ProtocolVersion &&
                               position.Wallet.Address == wallet.Address &&
                               position.ClosedAt == null);
    } 
}