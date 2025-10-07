using System.Runtime.CompilerServices;
using CryptoWatcher.UniswapModule;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;

public interface IUnichainLogProvider
{
    IAsyncEnumerable<FilterLog[]> GetLogsAsync(Web3 web3, ulong fromBlock, ulong toBlock,
        CancellationToken ct = default);
}

public class UnichainLogProvider : IUnichainLogProvider
{
    private const ulong ChunkSize = 100;

    public async IAsyncEnumerable<FilterLog[]> GetLogsAsync(Web3 web3,
        ulong fromBlock,
        ulong toBlock, [EnumeratorCancellation] CancellationToken ct = default)
    {
        for (var chunkStart = fromBlock; chunkStart <= toBlock; chunkStart += ChunkSize)
        {
            ct.ThrowIfCancellationRequested();

            var chunkEnd = Math.Min(chunkStart + ChunkSize - 1, toBlock);

            var filter = new NewFilterInput
            {
                FromBlock = new BlockParameter(chunkStart),
                ToBlock = new BlockParameter(chunkEnd),
                Topics =
                [
                    new[] { UnichainWellKnownField.V4ModifyLiquiditySignature },
                    null,
                    new[] { UnichainWellKnownField.V4PositionManagerAddress }
                ],
            };

            var logs = await web3.Eth.Filters.GetLogs.SendRequestAsync(filter);

            yield return logs;
        }
    }
}