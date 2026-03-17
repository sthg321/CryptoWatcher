using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Extensions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Abstractions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.EventLogs;
using CryptoWatcher.ValueObjects;
using Nethereum.Contracts;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.LogEventDecoders;

public class UniswapV3CollectLogEventDecoder : ITransactionLogEventDecoder
{
    /// <summary>
    /// There is 2 possible claim operating.
    /// 1. When it's just a regular commission claim/
    /// 2. When position close with commission. Then with DecreaseLiquidityEvent commission will be claimed too.
    /// </summary>
    /// <param name="transactionReceipt"></param>
    /// <returns></returns>
    public bool CanDecode(TransactionReceipt transactionReceipt)
    {
        return transactionReceipt.DecodeAllEvents<PoolCollectEventLog>().Count == 1 &&
               transactionReceipt.DecodeAllEvents<ManagerCollectEventLog>().Count == 1 &&
               transactionReceipt.DecodeAllEvents<DecreaseLiquidityEventLog>().Count == 0;
    }

    public PositionEvent GetOperation(TransactionReceipt transactionReceipt)
    {
        var collectEvents = transactionReceipt.DecodeAllEvents<ManagerCollectEventLog>().Single();

        var tokenTransfers = transactionReceipt.DecodeAllEvents<TransferEventDTO>();

        var (token0, token1) =
            tokenTransfers.MapEventToTokens(collectEvents.Event.Amount0, collectEvents.Event.Amount1);

        return new CollectFeesEvent
        {
            PositionId = (ulong)collectEvents.Event.TokenId,
            Token0 = token0,
            Token1 = token1,
            TransactionHash = transactionReceipt.TransactionHash,
            BlockNumber = transactionReceipt.BlockNumber,
            From = EvmAddress.Create(transactionReceipt.From)
        };
    }
}