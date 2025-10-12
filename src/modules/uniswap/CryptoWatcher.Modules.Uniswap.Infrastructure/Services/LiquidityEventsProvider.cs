using System.Numerics;
using System.Runtime.CompilerServices;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Models;
using Microsoft.Extensions.Logging;
using Polly.Registry;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;

internal class LiquidityEventsProvider : ILiquidityEventsProvider
{
    private readonly IBlockchainLogProvider _logProvider;
    private readonly ITransactionDataProvider _transactionDataProvider;
    private readonly ILiquidityPoolEventDecoder _eventDecoder;
    private readonly ILogger<LiquidityEventsProvider> _logger;
    private readonly ResiliencePipelineRegistry<string> _pipelineRegistry;

    public LiquidityEventsProvider(IBlockchainLogProvider logProvider, ITransactionDataProvider transactionDataProvider,
        ILiquidityPoolEventDecoder eventDecoder, ILogger<LiquidityEventsProvider> logger,
        ResiliencePipelineRegistry<string> pipelineRegistry)
    {
        _logProvider = logProvider;
        _transactionDataProvider = transactionDataProvider;
        _eventDecoder = eventDecoder;
        _logger = logger;
        _pipelineRegistry = pipelineRegistry;
    }

    public async IAsyncEnumerable<IEnumerable<LiquidityPoolPositionEvent>> FetchLiquidityPoolEvents(
        UniswapChainConfiguration chain,
        BigInteger fromBlock,
        BigInteger toBlock,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var blockchainLogEntries = await _logProvider.GetLogsAsync(chain, fromBlock, toBlock);

        var rateLimiter = _pipelineRegistry.GetPipeline("Uniswap");
        
        var tasks = blockchainLogEntries.Select(async log =>
        {
            try
            {
                var transactionData = await rateLimiter.ExecuteAsync<TransactionData?>(
                        async token =>
                            await _transactionDataProvider.GetTransactionDataAsync(chain, log.TransactionHash, token),
                        ct)
                    .ConfigureAwait(false);

                if (transactionData is null)
                {
                    return null;
                }

                return _eventDecoder.DecodeModifyLiquidityEvent(transactionData.WalletAddress, log.Data,
                    transactionData.TransactionHash,
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
            yield return events.Where(@event => @event is not null)!;
        }
    }
}