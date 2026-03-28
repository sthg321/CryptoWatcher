using CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Models;
using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Fluid.Infrastructure.Integrations.Blockchain.Abstractions;

public interface IFluidTransactionLogDecoder
{
    bool CanDecode(TransactionReceipt receipt);
    
    FluidEvent DecodeEventFromLog(TransactionReceipt receipt);
}