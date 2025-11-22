using CryptoWatcher.Modules.Uniswap.Application;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services.EventsSynchronization.V3;

internal class UniswapV3BlockchainLogProvider : BlockchainLogProviderBase, IBlockchainLogProvider
{
    public UniswapV3BlockchainLogProvider(IWeb3Factory web3Factory) : base(web3Factory)
    {
    }

    protected override object?[] GetLogSignatureFilter(UniswapChainConfiguration chainConfiguration)
    {
        return
        [
            new[]
            {
                UniswapWellKnownField.V3CollectSignature,
                // UniswapWellKnownField.V3MintSignature,
                // UniswapWellKnownField.V3DecreaseLiquiditySignature,
                // UniswapWellKnownField.V3IncreaseLiquiditySignature,
            },
            null,
            null
        ];
    }
}