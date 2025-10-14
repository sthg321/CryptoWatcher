using System.Numerics;

namespace CryptoWatcher.Modules.Uniswap.Abstractions;

public interface IChainLogChunkingStrategy
{
    IEnumerable<(BigInteger from, BigInteger to)> CreateChunks(BigInteger fromBlock, BigInteger toBlock);
}