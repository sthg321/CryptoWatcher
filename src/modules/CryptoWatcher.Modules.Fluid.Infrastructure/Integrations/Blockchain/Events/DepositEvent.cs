using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CryptoWatcher.Modules.Fluid.Infrastructure.Integrations.Blockchain.Events;

[Event("Deposit")]
public class DepositEvent : IEventDTO
{
    /// <summary>
    /// Индексированный параметр sender (address)
    /// </summary>
    [Parameter("address", "sender", 1, true)]
    public string Sender { get; set; } = null!;

    /// <summary>
    /// Индексированный параметр owner (address)
    /// </summary>
    [Parameter("address", "owner", 2, true)]
    public string Owner { get; set; } = null!;

    /// <summary>
    /// Неиндексированный параметр assets (uint256)
    /// </summary>
    [Parameter("uint256", "assets", 3, false)]
    public BigInteger Assets { get; set; }

    /// <summary>
    /// Неиндексированный параметр shares (uint256)
    /// </summary>
    [Parameter("uint256", "shares", 4, false)]
    public BigInteger Shares { get; set; }
}