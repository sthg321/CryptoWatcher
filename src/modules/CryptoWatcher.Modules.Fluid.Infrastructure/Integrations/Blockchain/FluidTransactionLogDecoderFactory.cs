using CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Models;
using CryptoWatcher.Modules.Fluid.Infrastructure.Integrations.Blockchain.Abstractions;
using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Fluid.Infrastructure.Integrations.Blockchain;

public class FluidTransactionLogDecoderFactory : IFluidTransactionLogDecoderFactory
{
    private readonly List<IFluidTransactionLogDecoder> _logDecoders;

    public FluidTransactionLogDecoderFactory(IEnumerable<IFluidTransactionLogDecoder> logDecoders)
    {
        _logDecoders = logDecoders.ToList();
    }

    public FluidEvent? DecodeEventFromLog(TransactionReceipt receipt)
    {
        return _logDecoders.FirstOrDefault(x => x.CanDecode(receipt))?.DecodeEventFromLog(receipt);
    }
}