using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Extensions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Abstractions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Models.Events;
using Nethereum.Contracts;
using Nethereum.Contracts.Standards.ERC721.ContractDefinition;
using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.LogEventDecoders;

public class UniswapV3MintLogEventDecoder : ITransactionLogEventDecoder
{
    public bool CanDecode(TransactionReceipt transactionReceipt)
    {
        // only 1 mint event can occur at once
        // note: ERC721 transfer event not the same as ERC20 transfer event. ERC20 - token transfers, ERC721 - nft
        return transactionReceipt.DecodeAllEvents<TransferEventDTO>().Count == 1;
    }

    public PositionOperation GetOperation(TransactionReceipt transactionReceipt)
    {
        var nftTransfer = transactionReceipt.DecodeAllEvents<TransferEventDTO>().Single();

        var mintEvent = transactionReceipt.DecodeAllEvents<MintEvent>().Single();

        var tokenTransfers = transactionReceipt
            .DecodeAllEvents<Nethereum.Contracts.Standards.ERC20.ContractDefinition.TransferEventDTO>();

        return new MintPositionOperation
        {
            PositionId = (ulong)nftTransfer.Event.TokenId,
            TransactionHash = transactionReceipt.TransactionHash,
            BlockNumber = transactionReceipt.BlockNumber,
            TickLower = mintEvent.Event.TickLower,
            TickUpper = mintEvent.Event.TickUpper,
            Token0 = tokenTransfers[0].MapEventToToken(),
            Token1 = tokenTransfers[1].MapEventToToken()
        };
    }
}