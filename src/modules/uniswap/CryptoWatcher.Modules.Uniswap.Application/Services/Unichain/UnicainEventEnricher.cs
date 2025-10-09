using System.Numerics;
using System.Runtime.CompilerServices;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Extensions;
using CryptoWatcher.Modules.Uniswap.Entities;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Unichain;

public class CashFlowEventMatcher : ICashFlowEventMatcher
{
    private readonly ILiquidityEventsProvider _liquidityEventsProvider;
    private readonly ITokenEnricher _tokenEnricher;
    private readonly ILogger<CashFlowEventMatcher> _logger;

    public CashFlowEventMatcher(ILiquidityEventsProvider liquidityEventsProvider, ITokenEnricher tokenEnricher,
        ILogger<CashFlowEventMatcher> logger)
    {
        _liquidityEventsProvider = liquidityEventsProvider;
        _tokenEnricher = tokenEnricher;
        _logger = logger;
    }

    public async IAsyncEnumerable<List<PoolPositionCashFlow>> FetchCashFlowEvents(
        UniswapChainConfiguration chainConfiguration,
        BigInteger fromBlock,
        BigInteger toBlock,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var events in _liquidityEventsProvider.FetchLiquidityPoolEvents(chainConfiguration,
                           fromBlock, toBlock, ct))
        {
            var result = new List<PoolPositionCashFlow>();

            foreach (var poolPositionEvent in events)
            {
                var enrichedTokenPair =
                    await _tokenEnricher.EnrichAsync(chainConfiguration.RpcUrl, poolPositionEvent.TokenPair, ct);

                var positionFromDb = chainConfiguration.LiquidityPoolPositions.SingleOrDefault(position =>
                {
                    var isTickMatch = position.TickLower == poolPositionEvent.TickLower &&
                                      position.TickUpper == poolPositionEvent.TickUpper;
                    if (!isTickMatch)
                    {
                        return false;
                    }

                    var normalizedPair = enrichedTokenPair.NormalizeToPositionOrder(position);

                    return position.Token0.Symbol == normalizedPair.Token0.Symbol &&
                           position.Token1.Symbol == normalizedPair.Token1.Symbol;
                });

                if (positionFromDb is null)
                {
                    _logger.LogDebug("No match for event ticks {TickLower}-{TickUpper}",
                        poolPositionEvent.TickLower, poolPositionEvent.TickUpper);
                    continue;
                }

                var cashFlow = PoolPositionCashFlow.CreateFromEvent(poolPositionEvent.Event,
                    positionFromDb.PositionId, chainConfiguration.Name, poolPositionEvent.TransactionHash,
                    enrichedTokenPair, poolPositionEvent.TimeStamp);
                
                result.Add(cashFlow);

                _logger.LogInformation("Matched cash flow for position {PositionId}", positionFromDb.PositionId);
            }

            yield return result;
        }
    }
}