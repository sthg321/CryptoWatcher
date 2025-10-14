using System.Numerics;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class ChainLogChunkingStrategy : IChainLogChunkingStrategy
{
    private static readonly BigInteger ChunkSize = new(100);

    private readonly ILogger<ChainLogChunkingStrategy>    _logger;

    public ChainLogChunkingStrategy(ILogger<ChainLogChunkingStrategy> logger)
    {
        _logger = logger;
    }

    public IEnumerable<(BigInteger from, BigInteger to)> CreateChunks(BigInteger fromBlock, BigInteger toBlock)
    {
        var blocksToProcess = toBlock - fromBlock;
        
        _logger.LogInformation("The number of block to process: {BlocksToProcess} ", blocksToProcess);
        
        var currentChunkStart = fromBlock;

        while (currentChunkStart <= toBlock)
        {
            var chunkEnd = currentChunkStart + ChunkSize - 1;
            if (chunkEnd > toBlock)
            {
                chunkEnd = toBlock;
            }
            
            yield return (currentChunkStart, chunkEnd);
            
            currentChunkStart = chunkEnd + 1;
        }
    }
}