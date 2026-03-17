using System.Numerics;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain.UniswapV3.LiquidityPool.Contracts;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain.UniswapV3.PositionsFetcher.Contracts;
using CryptoWatcher.ValueObjects;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Polly.Registry;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain.Api;

public interface IWeb3BlockchainApi
{
    Task<TransactionReceipt> GetTransactionReceiptAsync(UniswapChainConfiguration chain,
        TransactionHash transactionHash);

    Task<DateTime> GetTransactionTimestampAsync(UniswapChainConfiguration chain, BigInteger blockNumber);

    Task<BigInteger> GetPositionLiquidityAsync(UniswapChainConfiguration chain, BigInteger tokenId);
}

public class Web3BlockchainApi : IWeb3BlockchainApi
{
    private readonly IWeb3Factory _web3Factory;

    public Web3BlockchainApi(IWeb3Factory web3Factory)
    {
        _web3Factory = web3Factory;
    }

    public async Task<TransactionReceipt> GetTransactionReceiptAsync(UniswapChainConfiguration chain,
        TransactionHash transactionHash)
    {
        var web3 = _web3Factory.GetWeb3(chain);

        return await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
    }

    public async Task<DateTime> GetTransactionTimestampAsync(UniswapChainConfiguration chain, BigInteger blockNumber)
    {
        var web3 = _web3Factory.GetWeb3(chain);

        var result = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(
                new BlockParameter(blockNumber.ToHexBigInteger()));

        return DateTimeOffset.FromUnixTimeSeconds((long)result.Timestamp.Value).UtcDateTime;
    }

    public async Task<BigInteger> GetPositionLiquidityAsync(UniswapChainConfiguration chain, BigInteger tokenId)
    {
        var web3 = _web3Factory.GetWeb3(chain);

        var handler = web3.Eth
            .GetContractQueryHandler<PositionsFunction>();

        var function = new PositionsFunction
        {
            TokenId = tokenId
        };

        var result = await handler.QueryDeserializingToObjectAsync<PositionsOutputDTO>(
            function,
            chain.SmartContractAddresses.PositionManager);

        return result.Liquidity;
    }

    public async Task<BigInteger> GetHistoricalSlot0(UniswapChainConfiguration chain, EvmAddress poolAddress,
        BigInteger blockNumber)
    {
        var web3 = _web3Factory.GetWeb3(chain);

        // ABI минимально необходимый для slot0
        const string poolAbi = """
                               [
                                 {
                                   "inputs": [],
                                   "name": "slot0",
                                   "outputs": [
                                     { "internalType": "uint160", "name": "sqrtPriceX96", "type": "uint160" },
                                     { "internalType": "int24",   "name": "tick",         "type": "int24" },
                                     { "internalType": "uint16",  "name": "observationIndex", "type": "uint16" },
                                     { "internalType": "uint16",  "name": "observationCardinality", "type": "uint16" },
                                     { "internalType": "uint16",  "name": "observationCardinalityNext", "type": "uint16" },
                                     { "internalType": "uint8",   "name": "feeProtocol",  "type": "uint8" },
                                     { "internalType": "bool",    "name": "unlocked",     "type": "bool" }
                                   ],
                                   "stateMutability": "view",
                                   "type": "function"
                                 }
                               ]
                               """;

        var contract = web3.Eth.GetContract(poolAbi, poolAddress.Value);

        var slot0Function = contract.GetFunction("slot0");

        var blockParameter = new BlockParameter(
            new HexBigInteger(blockNumber)
        );

        // slot0 возвращает tuple — читаем всё, но используем только sqrtPriceX96
        var result = await slot0Function
            .CallDeserializingToObjectAsync<Slot0OutputDto>(blockParameter);

        return result.SqrtPriceX96;
    }
}