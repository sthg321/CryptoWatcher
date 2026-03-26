using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Abstractions;

namespace CryptoWatcher.Modules.Fluid.Application.Features;

public class FluidTransactionClassifier : IFluidTransactionClassifier
{
    private static readonly HashSet<string> FluidLendFunctionNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "deposit"
    };

    private readonly IFluidLendAddressCache _addressCache;

    public FluidTransactionClassifier(IFluidLendAddressCache addressCache)
    {
        _addressCache = addressCache;
    }

    public bool IsFluidLendTransactionAsync(BlockchainTransaction transaction)
    {
        return transaction.FunctionName is null ||
               FluidLendFunctionNames.Any(functionName => transaction.FunctionName.Contains(functionName)) ||
               _addressCache.GetAddress(transaction.From) is not null;
    }
}