using CryptoWatcher.Modules.Contracts.Messages;

namespace CryptoWatcher.Modules.Fluid.Application.Features.Abstractions;

public interface IFluidTransactionConsumer
{
    Task ConsumeAsync(BlockchainTransaction transaction, CancellationToken ct = default);
}