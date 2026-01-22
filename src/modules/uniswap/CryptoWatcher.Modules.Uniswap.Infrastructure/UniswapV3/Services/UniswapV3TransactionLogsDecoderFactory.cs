using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
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

    public PositionOperation? GetOperationFromTransaction(TransactionReceipt transactionReceipt)
    {
        var decoder = _logsDecoders.FirstOrDefault(x => x.CanDecode(transactionReceipt));

        return decoder?.GetOperation(transactionReceipt);
    }
}