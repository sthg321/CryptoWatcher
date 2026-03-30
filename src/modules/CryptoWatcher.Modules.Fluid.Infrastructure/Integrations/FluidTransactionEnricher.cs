using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.Fluid.Application.Features.Abstractions;
using CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Abstractions;
using CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Models;
using CryptoWatcher.Modules.Fluid.Infrastructure.Integrations.Blockchain.Abstractions;
using CryptoWatcher.Modules.Infrastructure.Shared.Integrations.Abstractions;

namespace CryptoWatcher.Modules.Fluid.Infrastructure.Integrations;

public class FluidTransactionEnricher : IFluidTransactionEnricher
{
    private readonly IFluidTransactionClassifier _transactionClassifier;
    private readonly IFluidTransactionLogDecoderFactory _decoderFactory;
    private readonly IWeb3Gateway _gateway;

    public FluidTransactionEnricher(IFluidTransactionClassifier transactionClassifier,
        IFluidTransactionLogDecoderFactory decoderFactory, IWeb3Gateway gateway)
    {
        _transactionClassifier = transactionClassifier;
        _decoderFactory = decoderFactory;
        _gateway = gateway;
    }

    public async ValueTask<FluidEventDetails?> EnrichAsync(BlockchainTransaction transaction,
        CancellationToken ct = default)
    {
        if (!_transactionClassifier.IsFluidLendTransactionAsync(transaction))
        {
            return null;
        }

        var web3 = _gateway.GetConfigured(transaction.ChainId);

        var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transaction.Hash.Value);

        var fluidEvent = _decoderFactory.DecodeEventFromLog(receipt);

        if (fluidEvent is null)
        {
            return null;
        }
        
        var block = await web3.Eth.Blocks.GetBlockWithTransactionsByHash.SendRequestAsync(receipt.BlockHash);
        
        return new FluidEventDetails
        {
            ChainId = transaction.ChainId,
            Event = fluidEvent,
            Hash = transaction.Hash,
            Timestamp = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).UtcDateTime,
            WalletAddress = transaction.From
        };
    }
}