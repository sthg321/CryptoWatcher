using System.Numerics;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models
    .PositionEvents;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Extensions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.
    Abstractions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.
    Models.EventLogs;
using CryptoWatcher.ValueObjects;
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

        var collectEvent = transactionReceipt.DecodeAllEvents<ManagerCollectEventLog>().Single();

        var tokenTransfers = transactionReceipt.DecodeAllEvents<TransferEventDTO>();

        var (token0, token1) = IsSingleTokenLeft(tokenTransfers)
            ? GetSingleToken(tokenTransfers, collectEvent)
            : tokenTransfers.MapEventToTokens(decreaseEvent.Event.Amount0, decreaseEvent.Event.Amount1);

        return new DecreaseLiquidityEvent
        {
            PositionId = (ulong)decreaseEvent.Event.TokenId,
            Token0 = token0,
            Token1 = token1,
            Commission0 = collectEvent.Event.Amount0 != 0 ? collectEvent.Event.Amount0 - token0?.Balance ?? 0 : 0,
            Commission1 = collectEvent.Event.Amount1 != 0 ? collectEvent.Event.Amount1 - token1?.Balance ?? 0 : 0,
            TransactionHash = transactionReceipt.TransactionHash,
            BlockNumber = transactionReceipt.BlockNumber,
            IsPositionClosed = decreaseEvent.Event.Amount0 == 0,
            From = EvmAddress.Create(transactionReceipt.From)
        };
    }

    private static bool IsSingleTokenLeft(List<EventLog<TransferEventDTO>> logs)
    {
        return logs[0].Log.Address == logs[1].Log.Address;
    }

    private static (Token? token0, Token? token1) GetSingleToken(List<EventLog<TransferEventDTO>> logs,
        EventLog<ManagerCollectEventLog> collectEvents)
    {
        return collectEvents.Event.Amount0 != 0
            ? (logs[0].MapEventToToken(), null)
            : (null, logs[0].MapEventToToken());
    }
}