using CryptoWatcher.Modules.Uniswap.Application;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services.EventsSynchronization.V4;

internal class UniswapV4BlockchainLogProvider : BlockchainLogProviderBase, IBlockchainLogProvider
{
    public UniswapV4BlockchainLogProvider(IWeb3Factory web3Factory) : base(web3Factory)
    {
    }

    protected override object?[] GetLogSignatureFilter(UniswapChainConfiguration chainConfiguration)
    {
        return
        [
            new[] { UniswapWellKnownField.V4ModifyLiquiditySignature },
            null,
            new[] { chainConfiguration.SmartContractAddresses.PositionManager.ToPaddedAddress() }
        ];
    }
}