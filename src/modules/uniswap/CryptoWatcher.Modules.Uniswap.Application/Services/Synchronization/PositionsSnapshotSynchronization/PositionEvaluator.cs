using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsSnapshotSynchronization.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsSnapshotSynchronization;

public class PositionEvaluator : IPositionEvaluator
{
    private readonly IUniswapMath _uniswapMath;
    private readonly ITokenEnricher _tokenEnricher;

    public PositionEvaluator(IUniswapMath uniswapMath, ITokenEnricher tokenEnricher)
    {
        _uniswapMath = uniswapMath;
        _tokenEnricher = tokenEnricher;
    }

    public async Task<PositionValuation> EvaluatePositionAsync(UniswapChainConfiguration chain,
        IUniswapPosition uniswapPosition, LiquidityPool pool,
        CancellationToken ct = default)
    {
        var positionInPool = _uniswapMath.CalculatePosition(pool, uniswapPosition);

        var fee = _uniswapMath.CalculateClaimableFee(pool, uniswapPosition);
        
        return new PositionValuation
        {
            IsInRange = positionInPool.IsInRange,
            PositionTokens = await EnrichTokenPair(chain, positionInPool.TokenInfoPair, ct),
            PositionFees = await EnrichTokenPair(chain, fee, ct)
        };
    }

    private async Task<TokenInfoPair> EnrichTokenPair(UniswapChainConfiguration chain, TokenPair tokenPair,
        CancellationToken ct)
    {
        return await _tokenEnricher.EnrichAsync(chain.Name,
            chain.RpcUrlWithAuthToken,
            tokenPair, ct);
    }
}