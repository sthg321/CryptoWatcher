using System.Numerics;
using System.Runtime.CompilerServices;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Models;
using Microsoft.Extensions.Logging;
using Polly.Registry;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;

internal class LiquidityEventsProvider : ILiquidityEventsProvider
{
    private readonly IEnumerable<IBlockchainLogProvider> _logProviders;
    private readonly ITransactionDataProvider _transactionDataProvider;
    private readonly IUniswapLiquidityPoolEventDecoderSelector _eventDecoderSelector;
    private readonly ResiliencePipelineRegistry<string> _pipelineRegistry;
    private readonly ILogger<LiquidityEventsProvider> _logger;

    public LiquidityEventsProvider(IEnumerable<IBlockchainLogProvider> logProviders,
        ITransactionDataProvider transactionDataProvider,
        IUniswapLiquidityPoolEventDecoderSelector eventDecoderSelector,
        ResiliencePipelineRegistry<string> pipelineRegistry,
        ILogger<LiquidityEventsProvider> logger)
    {
        _logProviders = logProviders;
        _transactionDataProvider = transactionDataProvider;
        _logger = logger;
        _pipelineRegistry = pipelineRegistry;
        _eventDecoderSelector = eventDecoderSelector;
    }

    public async IAsyncEnumerable<IReadOnlyCollection<LiquidityPoolPositionEvent>> FetchLiquidityPoolEvents(
        UniswapChainConfiguration chain,
        BigInteger fromBlock,
        BigInteger toBlock,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var getLogsTasks = _logProviders.Select(provider => provider.GetLogsAsync(chain, fromBlock, toBlock)).ToArray();

        await Task.WhenAll(getLogsTasks);

        var blockchainLogEntries = getLogsTasks.SelectMany(task => task.Result).ToList();

        _logger.LogInformation("Found {LogsCount} logs", blockchainLogEntries.Count);

        var rateLimiter = _pipelineRegistry.GetPipeline("Uniswap");

        var tasks = blockchainLogEntries.Select(async log =>
        {
            try
            {
                var transactionData = await rateLimiter.ExecuteAsync<TransactionData?>(async token =>
                    await _transactionDataProvider.GetTransactionDataAsync(chain, log.TransactionHash, token), ct);

                if (transactionData is null)
                {
                    return null;
                }

                return _eventDecoderSelector.DecodeEvent(transactionData.WalletAddress, log,
                    transactionData.EventEnrichment.TokenPair,
                    transactionData.EventEnrichment.TimeStamp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process log {TransactionHash}", log.TransactionHash);
                return null;
            }
        });

        foreach (var taskChunk in tasks.Chunk(20))
        {
            var events = await Task.WhenAll(taskChunk);
            yield return events.Where(@event => @event is not null).ToArray()!;
        }
    }
}