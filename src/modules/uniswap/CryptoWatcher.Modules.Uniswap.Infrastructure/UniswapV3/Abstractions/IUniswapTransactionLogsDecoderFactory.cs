using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Abstractions;

public interface IUniswapTransactionLogsDecoderFactory
{
    PositionOperation? GetOperationFromTransaction(TransactionReceipt transactionReceipt);
}