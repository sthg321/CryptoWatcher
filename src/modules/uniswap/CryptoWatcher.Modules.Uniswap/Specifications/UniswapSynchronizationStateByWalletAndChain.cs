using Ardalis.Specification;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Uniswap.Specifications;

public class UniswapSynchronizationStateByWalletAndChain : Specification<UniswapSynchronizationState>,
    ISingleResultSpecification<UniswapSynchronizationState>
{
    public UniswapSynchronizationStateByWalletAndChain(UniswapChainConfiguration chainConfiguration, Wallet wallet)
    {
        Query.Where(state => state.ChainName == chainConfiguration.Name && state.WalletAddress == wallet.Address);
    }
}