using System.Numerics;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Models;

public class EtherscanTransactionQuery
{
    public EvmAddress WalletAddress { get; init; } = null!;

    public int ChainId { get; init; }

    public string ApiKey { get; init; } = null!;

    public int Page { get; init; }

    public int Offset { get; init; }

    public BigInteger StartBlock { get; init; }
}