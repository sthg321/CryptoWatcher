using CryptoWatcher.Modules.Uniswap.Application.Abstractions.OperationReaders;
using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.UniswapV3.OperationReaders;

public class PositionOperationOrchestrator
{
    private readonly IPositionOperationApplierFactory _operationApplierFactory;
    private readonly IMintPositionOperationApplier _mintPositionOperationApplier;

    public PositionOperationOrchestrator(IPositionOperationApplierFactory operationApplierFactory,
        IMintPositionOperationApplier mintPositionOperationApplier)
    {
        _operationApplierFactory = operationApplierFactory;
        _mintPositionOperationApplier = mintPositionOperationApplier;
    }

    public async Task<UniswapLiquidityPosition> ApplyOperationToPositionAsync(
        EvmAddress walletAddress,
        UniswapChainConfiguration chainConfiguration,
        PositionOperationInfo operationInfo,
        UniswapLiquidityPosition? position,
        CancellationToken ct = default)
    {
        if (operationInfo.Operation is MintPositionOperation mintPositionOperation)
        {
            return await _mintPositionOperationApplier.ReadOperationAsync(walletAddress, mintPositionOperation,
                chainConfiguration, operationInfo.OperationDate, ct);
        }

        var applier = _operationApplierFactory.GetOperationApplier(operationInfo.Operation);

        return await applier.ApplyOperationAsync(chainConfiguration, position!, operationInfo.Operation,
            operationInfo.OperationDate, ct);
    }
}