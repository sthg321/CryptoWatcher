using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Abstractions;

internal interface IUniswapTransactionLogsDecoderFactory
{
    ITransactionLogEventDecoder? FindDecoder(TransactionReceipt transactionReceipt);
}