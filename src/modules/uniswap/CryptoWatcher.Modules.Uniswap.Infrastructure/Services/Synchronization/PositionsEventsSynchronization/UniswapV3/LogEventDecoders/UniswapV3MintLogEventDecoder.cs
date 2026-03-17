using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Extensions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Abstractions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.EventLogs;
using CryptoWatcher.ValueObjects;
using Nethereum.Contracts;
using Nethereum.Contracts.Standards.ERC721.ContractDefinition;
using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.LogEventDecoders;

public class UniswapV3MintLogEventDecoder : ITransactionLogEventDecoder
{
    public bool CanDecode(TransactionReceipt transactionReceipt)
    {
        // only 1 mint event can occur at once
        // note: ERC721 transfer event not the same as ERC20 transfer event. ERC20 - token transfers, ERC721 - nft
        return transactionReceipt.DecodeAllEvents<TransferEventDTO>().Count == 1;
    }

    public PositionEvent GetOperation(TransactionReceipt transactionReceipt)
    {
        var nftTransfer = transactionReceipt.DecodeAllEvents<TransferEventDTO>().Single();

        var mintEvent = transactionReceipt.DecodeAllEvents<MintEventLog>().Single();

        var tokenTransfers = transactionReceipt
            .DecodeAllEvents<Nethereum.Contracts.Standards.ERC20.ContractDefinition.TransferEventDTO>();

        var (token0, token1) = tokenTransfers.MapEventToTokens();

        return new MintPositionEvent
        {
            PositionId = (ulong)nftTransfer.Event.TokenId,
            TransactionHash = transactionReceipt.TransactionHash,
            PollAddress = EvmAddress.Create(mintEvent.Log.Address),
            BlockNumber = transactionReceipt.BlockNumber,
            TickLower = mintEvent.Event.TickLower,
            TickUpper = mintEvent.Event.TickUpper,
            Token0 = token0,
            Token1 = token1,
            From = EvmAddress.Create(transactionReceipt.From)
        };
    }
}