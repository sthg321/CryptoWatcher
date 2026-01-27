using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Abstractions;

public interface ITransactionLogEventDecoder
{
    bool CanDecode(TransactionReceipt transactionReceipt);
    
    PositionEvent GetOperation(TransactionReceipt transactionReceipt);
}