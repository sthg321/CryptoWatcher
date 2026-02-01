using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Extensions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Abstractions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.EventLogs;
using Nethereum.Contracts;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization.UniswapV3
    .LogEventDecoders;

public class UniswapV3DecreaseLiquidityLogEventDecoder : ITransactionLogEventDecoder
{
    public bool CanDecode(TransactionReceipt transactionReceipt)
    {
        return transactionReceipt.DecodeAllEvents<DecreaseLiquidityEventLog>().Count == 1;
    }

    public PositionEvent GetOperation(TransactionReceipt transactionReceipt)
    {
        var decreaseEvent = transactionReceipt.DecodeAllEvents<DecreaseLiquidityEventLog>().Single();

        var collectEvents = transactionReceipt.DecodeAllEvents<ManagerCollectEventLog>().Single();

        var tokenTransfers = transactionReceipt.DecodeAllEvents<TransferEventDTO>();

        var (token0, token1) =
            tokenTransfers.MapEventToTokens(decreaseEvent.Event.Amount0, decreaseEvent.Event.Amount1);

        return new DecreaseLiquidityEvent
        {
            PositionId = (ulong)decreaseEvent.Event.TokenId,
            Token0 = token0,
            Token1 = token1,
            Commission0 = collectEvents.Event.Amount0 != 0 ? collectEvents.Event.Amount0 - token1.Balance : 0,
            Commission1 = collectEvents.Event.Amount1 != 0 ? collectEvents.Event.Amount1 - token1.Balance : 0,
            TransactionHash = transactionReceipt.TransactionHash,
            BlockNumber = transactionReceipt.BlockNumber,
            IsPositionClosed = decreaseEvent.Event.Amount0 == 0
        };
    }
}