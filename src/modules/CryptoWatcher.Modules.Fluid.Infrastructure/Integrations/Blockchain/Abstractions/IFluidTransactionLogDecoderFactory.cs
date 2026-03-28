using CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Models;
using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Fluid.Infrastructure.Integrations.Blockchain.Abstractions;

public interface IFluidTransactionLogDecoderFactory
{
    FluidEvent? DecodeEventFromLog(TransactionReceipt receipt);
}