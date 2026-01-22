using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions.OperationReaders;
using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.UniswapV3.OperationReaders;

public abstract class BasePositionOperationApplier<TOperation> : IPositionMutationOperation
    where TOperation : PositionOperation
{
    private readonly ITokenEnricher _tokenEnricher;

    protected BasePositionOperationApplier(ITokenEnricher tokenEnricher)
    {
        _tokenEnricher = tokenEnricher;
    }

    public async Task<UniswapLiquidityPosition> ApplyOperationAsync(
        UniswapChainConfiguration chainConfiguration,
        UniswapLiquidityPosition position,
        PositionOperation operation,
        DateTime timestamp,
        CancellationToken ct = default)
    {
        var enrichedTokens = await EnrichTokensAsync(chainConfiguration, operation.Token0, operation.Token1, ct);

        await ApplyOperation(chainConfiguration, position, enrichedTokens, (TOperation)operation, timestamp);

        return position;
    }
    
    protected async Task<TokenInfoPair> EnrichTokensAsync(UniswapChainConfiguration chainConfiguration, Token token0,
        Token token1, CancellationToken ct = default)
    {
        return await _tokenEnricher.EnrichAsync(chainConfiguration.Name,
            chainConfiguration.RpcUrlWithAuthToken,
            new TokenPair
            {
                Token0 = token0,
                Token1 = token1
            }, ct);
    }
    
    protected abstract ValueTask ApplyOperation(
        UniswapChainConfiguration chainConfiguration,
        UniswapLiquidityPosition position,
        TokenInfoPair enrichedTokens,
        TOperation operation,
        DateTime timestamp);
}