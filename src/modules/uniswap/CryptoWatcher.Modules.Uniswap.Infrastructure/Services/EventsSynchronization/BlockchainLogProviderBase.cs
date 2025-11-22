using System.Numerics;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services.EventsSynchronization;

internal abstract class BlockchainLogProviderBase
{
    private readonly IWeb3Factory _web3Factory;

    public BlockchainLogProviderBase(IWeb3Factory web3Factory)
    {
        _web3Factory = web3Factory;
    }

    public async Task<IReadOnlyCollection<BlockchainLogEntry>> GetLogsAsync(
        UniswapChainConfiguration chainConfiguration, BigInteger fromBlock, BigInteger toBlock)
    {
        var web3 = _web3Factory.GetWeb3(chainConfiguration);

        var filter = new NewFilterInput
        {
            FromBlock = new BlockParameter(fromBlock.ToHexBigInteger()),
            ToBlock = new BlockParameter(toBlock.ToHexBigInteger()),
            Topics = GetLogSignatureFilter(chainConfiguration)
        };

        var logs = await web3.Eth.Filters.GetLogs.SendRequestAsync(filter);

        return logs.Select(log => new BlockchainLogEntry
            {
                Address = EvmAddress.Create(log.Address), Data = log.Data, TransactionHash = log.TransactionHash,
                Topics = log.Topics
            })
            .ToArray();
    }

    protected abstract object?[] GetLogSignatureFilter(UniswapChainConfiguration chainConfiguration);
}