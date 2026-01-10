using CryptoWatcher.Modules.Uniswap.Application;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Application.Services;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services.EventsSynchronization;

internal class LiquidityEventLogEnricher : ILiquidityEventLogEnricher
{
    private const int FirstTokenIndex = 1;
    private const int SecondTokenIndex = 2;

    private readonly IBlockscoutProvider _blockscoutProvider;

    private readonly ILogger<LiquidityEventLogEnricher> _logger;

    public LiquidityEventLogEnricher(IBlockscoutProvider blockscoutProvider,
        ILogger<LiquidityEventLogEnricher> logger)
    {
        _blockscoutProvider = blockscoutProvider;
        _logger = logger;
    }

    public async Task<LiquidityEventEnrichment?> EnrichLiquidityEventFromLogsAsync(
        UniswapChainConfiguration chain,
        EvmAddress walletAddress,
        TransactionHash transactionHash,
        LiquidityEventLog[] logs,
        CancellationToken ct = default)
    {
        return logs.Length switch
        {
            // Modify liquidity event occured in pool when 1 token is ETH.
            // Cuz ETH is a native token for unichain network, then it's not included in the logs cuz it's not the ERC-20 token.
            // So we need to manually call block scout to get the internal transaction-by-transaction hash
            2 => await CreateTokenPairFromLogsAndInternalTransactionAsync(chain, walletAddress, transactionHash, logs,
                ct),

            // For other cases there are 2 ERC-20 tokens in the pool. So we can get the event from the logs.
            3 => await CreateTokenPairFromLogs(chain, transactionHash, logs, ct),
            5 or 7 => await CreateTokenPairFromLogs(chain, transactionHash, logs, ct),
            _ => LogUnknownsLog(logs, transactionHash) // check later
        };
    }

    private async Task<LiquidityEventEnrichment> CreateTokenPairFromLogsAndInternalTransactionAsync(
        UniswapChainConfiguration chainConfiguration,
        EvmAddress walletAddress,
        TransactionHash transactionHash,
        LiquidityEventLog[] logs,
        CancellationToken ct)
    {
        _logger.LogInformation("Transaction {TransactionHash} has 2 logs and 1 is ETH", transactionHash);

        var token0 = CreateTokenFromLogs(logs, FirstTokenIndex);

        var ethAmount =
            await _blockscoutProvider.GetEthAmountFromInternalTransaction(chainConfiguration, walletAddress,
                transactionHash, ct);

        var token1 = new Token
        {
            Address = EvmAddress.Create(UniswapWellKnownField.EthAddress),
            Balance = ethAmount.Amount
        };

        return new LiquidityEventEnrichment
        {
            TimeStamp = ethAmount.TimeStamp,
            TokenPair = new TokenPair { Token0 = token0, Token1 = token1 }
        };
    }

    private async Task<LiquidityEventEnrichment> CreateTokenPairFromLogs(
        UniswapChainConfiguration chain,
        TransactionHash transactionHash,
        LiquidityEventLog[] logs,
        CancellationToken ct)
    {
        var timeStamp = await _blockscoutProvider.GetTransactionTimestampAsync(chain, transactionHash, ct);

        var token0 = CreateTokenFromLogs(logs, FirstTokenIndex);

        var token1 = CreateTokenFromLogs(logs, SecondTokenIndex);

        return new LiquidityEventEnrichment
        {
            TimeStamp = timeStamp,
            TokenPair = new TokenPair { Token0 = token0, Token1 = token1 }
        };
    }
    
    private async Task<LiquidityEventEnrichment> CreateTokenPairFromLogsV3(
        UniswapChainConfiguration chain,
        TransactionHash transactionHash,
        LiquidityEventLog[] logs,
        CancellationToken ct)
    {
        var timeStamp = await _blockscoutProvider.GetTransactionTimestampAsync(chain, transactionHash, ct);

        var logTopic = logs.Last();
        
        var token0 = CreateTokenFromLogs(logs, FirstTokenIndex);

        var token1 = CreateTokenFromLogs(logs, SecondTokenIndex);

        return new LiquidityEventEnrichment
        {
            TimeStamp = timeStamp,
            TokenPair = new TokenPair { Token0 = token0, Token1 = token1 }
        };
    }

    private LiquidityEventEnrichment? LogUnknownsLog(LiquidityEventLog[] logs, TransactionHash hash)
    {
        _logger.LogWarning(
            "Received unknown number of logs. Logs count: {LogsCount}. Transaction hash: Transaction hash: {TransactionHash}",
            logs.Length, hash);

        return null;
    }

    private static Token CreateTokenFromLogs(LiquidityEventLog[] logs, int index)
    {
        return new Token
        {
            Address = EvmAddress.Create(logs[index].Address),
            Balance = logs[index].Data
        };
    }
}