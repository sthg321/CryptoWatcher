using Ardalis.Specification;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Specifications;

public class GetUnichainSpecification : Specification<UniswapChainConfiguration>
{
    public GetUnichainSpecification(int chainId)
    {
        Query.Where(configuration => configuration.ChainId == chainId &&
                                     configuration.ProtocolVersion == UniswapProtocolVersion.V3);
    }
}