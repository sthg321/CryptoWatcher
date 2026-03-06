using System.Numerics;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.WalletIngestion.Application.Models;

public class EtherscanTransactionQuery
{
    public required EvmAddress Address { get; init; } = null!;

    public required int ChainId { get; init; }

    public required string ApiKey { get; init; } = null!;

    public required int Page { get; init; }

    public required int Offset { get; init; }

    public required BigInteger StartBlock { get; init; }
}