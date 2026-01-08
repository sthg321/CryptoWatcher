using System.Numerics;
using System.Runtime.CompilerServices;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Extensions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

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

    public async IAsyncEnumerable<List<UniswapLiquidityPositionCashFlow>> FetchCashFlowEvents(
        UniswapChainConfiguration chainConfiguration,
        BigInteger fromBlock,
        BigInteger toBlock,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var events in _liquidityEventsProvider.FetchLiquidityPoolEvents(chainConfiguration,
                           fromBlock, toBlock, ct))
        {
            _logger.LogInformation("Fetching {EventsCount} events", events.Count);

            var result = new List<UniswapLiquidityPositionCashFlow>();

            foreach (var poolPositionEvent in events)
            {
                _logger.LogInformation("Processing event with type: {EventType}", poolPositionEvent.Event.Name);

                var enrichedTokenPair =
                    await _tokenEnricher.EnrichAsync(
                        chainConfiguration.Name,
                        chainConfiguration.RpcUrlWithAuthToken,
                        poolPositionEvent.TokenPair, ct);

                var positionFromDb = chainConfiguration.LiquidityPoolPositions.SingleOrDefault(position =>
                {
                    if (!IsTickMatch(position, poolPositionEvent))
                    {
                        return false;
                    }

                    var normalizeToPositionOrder = enrichedTokenPair.NormalizeToPositionOrder(position);

                    if (normalizeToPositionOrder.Token0.Symbol != enrichedTokenPair.Token0.Symbol &&
                        normalizeToPositionOrder.Token1.Symbol != enrichedTokenPair.Token1.Symbol)
                    {
                        _logger.LogInformation(
                            "Tokens from event are not in the same order as in the position. Swap them");

                        enrichedTokenPair = normalizeToPositionOrder;
                    }

                    return IsSymbolMatch(position.Token0, enrichedTokenPair.Token0) &&
                           IsSymbolMatch(position.Token1, enrichedTokenPair.Token1);
                });

                if (positionFromDb is null)
                {
                    _logger.LogDebug("No match for event ticks {TickLower}-{TickUpper}",
                        poolPositionEvent.TickLower, poolPositionEvent.TickUpper);
                    continue;
                }

                _logger.LogInformation("Matched position {PositionId} for event ticks {TickLower}-{TickUpper}",
                    positionFromDb.PositionId, poolPositionEvent.TickLower, poolPositionEvent.TickUpper);

                var cashFlow = new UniswapLiquidityPositionCashFlow(
                    positionFromDb,
                    poolPositionEvent,
                    enrichedTokenPair,
                    poolPositionEvent.TimeStamp);

                result.Add(cashFlow);

                _logger.LogInformation("Matched cash flow for position {PositionId}", positionFromDb.PositionId);
            }

            yield return result;
        }
    }

    private static bool IsTickMatch(UniswapLiquidityPosition position, LiquidityPoolPositionEvent positionEvent)
    {
        return position.TickLower == positionEvent.TickLower &&
               position.TickUpper == positionEvent.TickUpper;
    }

    private static bool IsSymbolMatch(CryptoToken first, CryptoToken second)
    {
        return string.Equals(first.Symbol, second.Symbol, StringComparison.OrdinalIgnoreCase);
    }
}