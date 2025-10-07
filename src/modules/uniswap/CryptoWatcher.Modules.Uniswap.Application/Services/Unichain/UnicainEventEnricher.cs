using System.Runtime.CompilerServices;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Extensions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.UniswapModule.Entities;
using CryptoWatcher.UniswapModule.Services;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Unichain;

public class UnichainEventEnricher
{
    private readonly IUnichainEventFetcher _unichainEventFetcher;
    private readonly ITokenEnricher _tokenEnricher;
    private readonly IRepository<PoolPosition> _poolPositionRepository;
    private readonly ILastProcessedBlockNumberProvider _lastProcessedBlockNumberProvider;

    public UnichainEventEnricher(IUnichainEventFetcher unichainEventFetcher,
        ITokenEnricher tokenEnricher, IRepository<PoolPosition> poolPositionRepository,
        ILastProcessedBlockNumberProvider lastProcessedBlockNumberProvider)
    {
        _unichainEventFetcher = unichainEventFetcher;
        _tokenEnricher = tokenEnricher;
        _poolPositionRepository = poolPositionRepository;
        _lastProcessedBlockNumberProvider = lastProcessedBlockNumberProvider;
    }

    public async IAsyncEnumerable<List<PoolPositionCashFlow>> FetchCashFlowEvents(UniswapNetwork uniswapNetwork,
        string unichainRpc,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var lastBlock = await _lastProcessedBlockNumberProvider.GetLastProcessedBlockNumberAsync(ct);

        var walletToPositions = (await _poolPositionRepository.ListAsync(ct))
            .ToArray()
            .GroupBy(position => position.WalletAddress)
            .ToDictionary(position => position.Key,
                position => position.ToArray(), StringComparer.OrdinalIgnoreCase);

        await foreach (var events in _unichainEventFetcher.FetchLiquidityPoolEvents(unichainRpc, lastBlock, lastBlock,
                           ct))
        {
            var result = new List<PoolPositionCashFlow>();
 
            foreach (var poolPositionEvents in events.GroupBy(@event => @event.WalletAddress))
            {
                if (!walletToPositions.TryGetValue(poolPositionEvents.Key, out var dbPoolPositions))
                {
                    continue;
                }
                
                foreach (var poolPositionEvent in poolPositionEvents)
                {
                    var enrichedTokenPair =
                        await _tokenEnricher.EnrichAsync(unichainRpc, poolPositionEvent.TokenPair, ct);

                    var positionFromUniswap = dbPoolPositions.SingleOrDefault(position =>
                    {
                        // user can have in 1 wallet many positions with same tokens and tick but
                        // for now, we support only 1 position wih same ticks boundaries per wallet
                        var isTickMatch = position.TickLower == poolPositionEvent.TickLower &&
                                          position.TickUpper == poolPositionEvent.TickUpper;
                        if (!isTickMatch)
                        {
                            return false;
                        }

                        // cuz we can't know witch token is token0 and with is token1 from logs 
                        // we must swap them if they are not in the same order as in the db pool position
                        var swappedTokens = enrichedTokenPair.NormalizeToPositionOrder(position);

                        return position.Token0.Symbol == swappedTokens.Token0.Symbol &&
                               position.Token1.Symbol == swappedTokens.Token1.Symbol;
                    });

                    if (positionFromUniswap is null)
                    {
                        continue;
                    }

                    var @event = PoolPositionCashFlow.CreateFromEvent(poolPositionEvent.Event,
                        positionFromUniswap.PositionId, uniswapNetwork.Name, enrichedTokenPair);

                    result.Add(@event);
                }
            }
 
            yield return result;
        }
    }
}