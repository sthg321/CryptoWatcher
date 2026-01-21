using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Extensions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services;
using CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Abstractions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Models.Events;
using Nethereum.Contracts;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.LogEventDecoders;

public class UniswapV3DecreaseLiquidityLogEventDecoder : ITransactionLogEventDecoder
{
    public bool CanDecode(TransactionReceipt transactionReceipt)
    {
        return transactionReceipt.DecodeAllEvents<DecreaseLiquidityEvent>().Count == 1;
    }

    public PositionOperation GetOperation(TransactionReceipt transactionReceipt)
    {
        var decreaseEvent = transactionReceipt.DecodeAllEvents<DecreaseLiquidityEvent>().Single();

        var collectEvents = transactionReceipt.DecodeAllEvents<ManagerCollectEvent>().Single();

        var transferEvents = transactionReceipt.DecodeAllEvents<TransferEventDTO>();

        return new DecreaseLiquidityOperation
        {
            PositionId = (ulong)decreaseEvent.Event.TokenId,
            Token0 = transferEvents[0].MapEventToToken(decreaseEvent.Event.Amount0),
            Token1 = transferEvents[1].MapEventToToken(decreaseEvent.Event.Amount1),
            Commission0 = collectEvents.Event.Amount0,
            Commission1 = collectEvents.Event.Amount1,
            TransactionHash = transactionReceipt.TransactionHash,
            IsPositionClosed = decreaseEvent.Event.Amount0 == 0
        };
    }
}