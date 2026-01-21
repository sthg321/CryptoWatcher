using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Abstractions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Models.Events;
using CryptoWatcher.ValueObjects;
using Nethereum.Contracts;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.LogEventDecoders;

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
        return transactionReceipt.DecodeAllEvents<PoolCollectEvent>().Count == 1 &&
               transactionReceipt.DecodeAllEvents<ManagerCollectEvent>().Count == 1 &&
               transactionReceipt.DecodeAllEvents<DecreaseLiquidityEvent>().Count == 0;
    }

    public PositionOperation GetOperation(TransactionReceipt transactionReceipt)
    {
        var collectEvents = transactionReceipt.DecodeAllEvents<ManagerCollectEvent>().Single();

        var transferEvents = transactionReceipt.DecodeAllEvents<TransferEventDTO>();

        return new CollectFeesOperation
        {
            PositionId = (ulong)collectEvents.Event.TokenId,
            Token0 = new Token
            {
                Address = EvmAddress.Create(transferEvents[0].Log.Address),
                Balance = collectEvents.Event.Amount0
            },
            Token1 = new Token
            {
                Address = EvmAddress.Create(transferEvents[0].Log.Address),
                Balance = collectEvents.Event.Amount1
            },
            TransactionHash = transactionReceipt.TransactionHash
        };
    }
}