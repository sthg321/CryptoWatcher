using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions.OperationReaders;
using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.UniswapV3.OperationReaders;

public class MintPositionOperationApplier : IMintPositionOperationApplier
{
    private readonly ITokenEnricher _tokenEnricher;

    public MintPositionOperationApplier(ITokenEnricher tokenEnricher)
    {
        _tokenEnricher = tokenEnricher;
    }

    public async Task<UniswapLiquidityPosition> ReadOperationAsync(EvmAddress walletAddress,
        PositionOperationInfo mintPositionOperation,
        UniswapChainConfiguration chainConfiguration,
        CancellationToken ct = default)
    {
        var operation = mintPositionOperation.Operation as MintPositionOperation ??
                        throw new InvalidOperationException("Operation is not a MintPositionOperation");

        var enrichedTokens = await _tokenEnricher.EnrichAsync(chainConfiguration.Name,
            chainConfiguration.RpcUrlWithAuthToken,
            new TokenPair
            {
                Token0 = operation.Token0,
                Token1 = operation.Token1
            }, ct);

        var position = new UniswapLiquidityPosition(operation.PositionId,
            operation.TickLower,
            operation.TickUpper,
            enrichedTokens.Token0,
            enrichedTokens.Token1,
            walletAddress,
            chainConfiguration,
            DateOnly.FromDateTime(mintPositionOperation.OperationDate));

        return position;
    }
}