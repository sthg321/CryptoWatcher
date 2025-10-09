using System.Numerics;

namespace CryptoWatcher.Modules.Uniswap.Application.Models;

public class EthTransaction
{
    public BigInteger Amount { get; set; }

    public DateTimeOffset TimeStamp { get; set; }
}