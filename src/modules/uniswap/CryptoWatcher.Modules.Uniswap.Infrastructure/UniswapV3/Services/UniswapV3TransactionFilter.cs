using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Services;

public class UniswapV3TransactionFilter : IUniswapTransactionFilter
{
    private static readonly HashSet<string> V3LiquidityMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        "mint", "collect", "increaseLiquidity", "decreaseLiquidity", "burn", "multicall"
    };

    public bool IsRelevant(UniswapChainConfiguration config, BlockchainTransaction transaction)
    {
        return transaction.To.Equals(config.SmartContractAddresses.PositionManager) &&
               V3LiquidityMethods.Contains(transaction.FunctionName);
    }
}