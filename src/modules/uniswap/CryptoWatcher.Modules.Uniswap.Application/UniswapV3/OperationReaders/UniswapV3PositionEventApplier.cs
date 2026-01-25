using CryptoWatcher.Exceptions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions.OperationReaders;
using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.UniswapV3.OperationReaders;

public class UniswapV3PositionEventApplier : IUniswapPositionEventApplier
{
    private readonly IPositionOperationApplierFactory _operationApplierFactory;
    private readonly IMintPositionOperationApplier _mintPositionOperationApplier;

    public UniswapV3PositionEventApplier(IPositionOperationApplierFactory operationApplierFactory,
        IMintPositionOperationApplier mintPositionOperationApplier)
    {
        _operationApplierFactory = operationApplierFactory;
        _mintPositionOperationApplier = mintPositionOperationApplier;
    }

    public async Task<UniswapLiquidityPosition> ApplyOperationToPositionAsync(
        UniswapChainConfiguration chainConfiguration,
        EvmAddress walletAddress,
        UniswapEvent @event,
        UniswapLiquidityPosition? position, 
        CancellationToken ct = default)
    {
        if (@event.Operation is MintPositionOperation mintPositionOperation)
        {
            return await _mintPositionOperationApplier.CreatePositionAsync(walletAddress, mintPositionOperation,
                chainConfiguration, @event.Timestamp, ct);
        }

        if (position is null)
        {
            throw new DomainException("Position is not found for event");
        }

        if (position.IsClosed)
        {
            return position;
        }
        
        var applier = _operationApplierFactory.GetOperationApplier(@event.Operation);

        return await applier.ApplyOperationAsync(chainConfiguration, position!, @event.Operation,
            @event.Timestamp, ct);
    }
}