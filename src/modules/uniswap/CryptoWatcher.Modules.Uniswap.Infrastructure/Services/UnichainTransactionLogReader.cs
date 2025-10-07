using CryptoWatcher.Modules.Uniswap.Application.Services.Unichain;
using CryptoWatcher.Shared.ValueObjects;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;

public interface IUnichainLogReader
{
    Task<TokenPair> ReadTokenPairFromLogAsync(
        string transactionHash, FilterLog[] logs, CancellationToken ct = default);
}

internal class UnichainLogReader : IUnichainLogReader
{
    private readonly IUnichainInternalTransactionProvider _internalTransactionProvider;

    private const int FirstTokenIndex = 1;
    private const int SecondTokenIndex = 2;

    public UnichainLogReader(IUnichainInternalTransactionProvider internalTransactionProvider)
    {
        _internalTransactionProvider = internalTransactionProvider;
    }

    public async Task<TokenPair> ReadTokenPairFromLogAsync(
        string transactionHash, FilterLog[] logs, CancellationToken ct = default)
    {
        return logs.Length switch
        {
            // Modify liquidity event occured in pool when 1 token is ETH.
            // Cuz ETH is a native token for unichain network, then it's not included in the logs cuz it's not the ERC-20 token.
            // So we need to manually call block scout to get the internal transaction-by-transaction hash
            2 => await CreateTokenPairFromLogsAndInternalTransactionAsync(transactionHash, logs, ct),

            // For other cases there are 2 ERC-20 tokens in the pool. So we can get the event from the logs.
            3 => CreateTokenPairFromLogs(logs),
            _ => throw new InvalidOperationException(
                $"Unknown case for logs length. Logs length:{logs.Length}. Transaction hash: {transactionHash}")
        };
    }

    private async Task<TokenPair> CreateTokenPairFromLogsAndInternalTransactionAsync(string transactionHash,
        FilterLog[] logs,
        CancellationToken ct)
    {
        var token0 = CreateTokenFromLogs(logs, FirstTokenIndex);

        var ethAmount =
            await _internalTransactionProvider.GetEthAmountFromInternalTransaction(transactionHash, ct);
        var token1 = new Token
        {
            Address = UnichainWellKnownField.EthAddressInUnichain,
            Balance = ethAmount
        };

        return new TokenPair { Token0 = token0, Token1 = token1 };
    }

    private static TokenPair CreateTokenPairFromLogs(FilterLog[] logs)
    {
        var token0 = CreateTokenFromLogs(logs, FirstTokenIndex);

        var token1 = CreateTokenFromLogs(logs, SecondTokenIndex);

        return new TokenPair { Token0 = token0, Token1 = token1 };
    }

    private static Token CreateTokenFromLogs(FilterLog[] logs, int index)
    {
        return new Token
        {
            Address = logs[index].Address,
            Balance = logs[index].Data.HexToBigInteger(false)
        };
    }
}