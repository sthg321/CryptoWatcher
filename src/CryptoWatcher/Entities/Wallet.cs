namespace CryptoWatcher.Shared.Entities;

/// <summary>
/// Represents a cryptocurrency wallet entity within the system.
/// A wallet is defined by a unique identifier and an associated address.
/// </summary>
public class Wallet : IEquatable<Wallet>
{
    /// <summary>
    /// Gets the address associated with the wallet.
    /// This address is a unique identifier for blockchain interactions, allowing users or systems
    /// to send and receive cryptocurrency or interact with decentralized applications.
    /// </summary>
    public required string Address { get; init; } = null!;

    public bool Equals(Wallet? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Address == other.Address;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Wallet)obj);
    }

    public override int GetHashCode()
    {
        return Address.GetHashCode();
    }
}