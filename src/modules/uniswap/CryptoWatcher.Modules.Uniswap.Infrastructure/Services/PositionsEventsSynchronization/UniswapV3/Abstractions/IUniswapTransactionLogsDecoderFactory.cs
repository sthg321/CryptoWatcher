using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Abstractions;

public interface IUniswapTransactionLogsDecoderFactory
{
    PositionEvent? DecodeEventFromTransaction(TransactionReceipt transactionReceipt);
}