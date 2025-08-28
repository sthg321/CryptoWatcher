namespace CryptoWatcher.Shared.Entities;

/// <summary>
/// Represents a cryptocurrency wallet entity within the system.
/// A wallet is defined by a unique identifier and an associated address.
/// </summary>
public class Wallet
{
    /// <summary>
    /// Gets the address associated with the wallet.
    /// This address is a unique identifier for blockchain interactions, allowing users or systems
    /// to send and receive cryptocurrency or interact with decentralized applications.
    /// </summary>
    public required string Address { get; init; } = null!;
}