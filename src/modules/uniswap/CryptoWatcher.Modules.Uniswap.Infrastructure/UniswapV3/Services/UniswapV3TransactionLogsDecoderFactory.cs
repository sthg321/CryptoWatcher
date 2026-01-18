using CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Abstractions;
using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Services;

public class UniswapV3TransactionLogsDecoderFactory : IUniswapTransactionLogsDecoderFactory
{
    private readonly IEnumerable<ITransactionLogEventDecoder> _logsDecoders;

    public UniswapV3TransactionLogsDecoderFactory(IEnumerable<ITransactionLogEventDecoder> logsDecoders)
    {
        _logsDecoders = logsDecoders;
    }

    public ITransactionLogEventDecoder? FindDecoder(TransactionReceipt transactionReceipt)
    {
        return _logsDecoders.FirstOrDefault(x => x.CanDecode(transactionReceipt));
    }
}