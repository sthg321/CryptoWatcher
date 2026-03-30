using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.Fluid.Application.Features.Abstractions;
using CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization;

namespace CryptoWatcher.Modules.Fluid.Application.Features;

public class FluidTransactionConsumer : IFluidTransactionConsumer
{
    private readonly IFluidTransactionEnricher _transactionEnricher;
    private readonly FluidDepositEventHandler _eventHandler;

    public FluidTransactionConsumer(IFluidTransactionEnricher transactionEnricher,
        FluidDepositEventHandler eventHandler)
    {
        _transactionEnricher = transactionEnricher;
        _eventHandler = eventHandler;
    }

    public async Task ConsumeAsync(BlockchainTransaction transaction, CancellationToken ct = default)
    {
        var fluidTransaction = await _transactionEnricher.EnrichAsync(transaction, ct);

        if (fluidTransaction is null)
        {
            return;
        }

        await _eventHandler.HandleAsync(fluidTransaction);
    }
}