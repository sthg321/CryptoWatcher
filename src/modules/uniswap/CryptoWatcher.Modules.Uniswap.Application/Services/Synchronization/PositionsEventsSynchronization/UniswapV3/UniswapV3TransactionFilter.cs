using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3;

public class UniswapV3TransactionFilter : IUniswapTransactionFilter
{
    private static readonly HashSet<string> V3LiquidityMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        "mint", "collect", "increaseLiquidity", "decreaseLiquidity", "burn", "multicall"
    };

    public bool IsRelevant(UniswapChainConfiguration config, BlockchainTransaction transaction)
    {
        var addresses = config.SmartContractAddressesList.FirstOrDefault(uniswapAddresses =>
            uniswapAddresses.ProtocolVersion == UniswapProtocolVersion.V3);

        if (addresses is null)
        {
            throw new InvalidOperationException("No Uniswap v3 addresses found");
        }        

        return transaction.FunctionName is null ||
               V3LiquidityMethods.Any(functionName => transaction.FunctionName.Contains(functionName));
    }
}