using CryptoWatcher.Modules.Contracts.Messages;

namespace CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Abstractions;

public interface IFluidTransactionClassifier
{
    bool IsFluidLendTransactionAsync(BlockchainTransaction transaction);
}