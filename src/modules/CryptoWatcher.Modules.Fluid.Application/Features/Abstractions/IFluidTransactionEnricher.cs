using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Models;

namespace CryptoWatcher.Modules.Fluid.Application.Features.Abstractions;

public interface IFluidTransactionEnricher
{
    ValueTask<FluidEventDetails?> EnrichAsync(BlockchainTransaction transaction, CancellationToken ct = default);
}