using System.Runtime.CompilerServices;
using CryptoWatcher.UniswapModule.Models;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using Nethereum.Web3;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;


public class UnichainEventFetcher : IUnichainEventFetcher
{
    private readonly IUnichainLogProvider _unichainLogProvider;
    private readonly ILiquidityPoolEventDecoder _liquidityPoolEventDecoder;
    private readonly IUnichainLogReader _unichainLogReader;

    public UnichainEventFetcher(
        IUnichainLogProvider unichainLogProvider,
        ILiquidityPoolEventDecoder liquidityPoolEventDecoder, IUnichainLogReader unichainLogReader)
    {
        _unichainLogProvider = unichainLogProvider;
        _liquidityPoolEventDecoder = liquidityPoolEventDecoder;
        _unichainLogReader = unichainLogReader;
    }

    public async IAsyncEnumerable<List<LiquidityPoolPositionEvent>> FetchLiquidityPoolEvents(string unichainRpc,
        ulong fromBlock,
        ulong toBlock,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var web3 = new Web3(unichainRpc);

        await foreach (var unichainLogsBatch in _unichainLogProvider.GetLogsAsync(web3, fromBlock, toBlock, ct))
        {
            var result = new List<LiquidityPoolPositionEvent>(unichainLogsBatch.Length);

            foreach (var log in unichainLogsBatch)
            {
                var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(log.TransactionHash);

                var tokenPair =
                    await _unichainLogReader.ReadTokenPairFromLogAsync(log.TransactionHash, receipt.Logs, ct);

                var @event =
                    _liquidityPoolEventDecoder.DecodeModifyLiquidityEvent(receipt.From.ToLowerInvariant(), log.Data,
                        tokenPair);

                result.Add(@event);
            }

            yield return result;
        }
    }
}