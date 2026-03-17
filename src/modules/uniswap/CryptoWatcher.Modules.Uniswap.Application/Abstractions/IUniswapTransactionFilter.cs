using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IUniswapTransactionFilter
{
    bool IsRelevant(UniswapChainConfiguration config, BlockchainTransaction transaction);
}