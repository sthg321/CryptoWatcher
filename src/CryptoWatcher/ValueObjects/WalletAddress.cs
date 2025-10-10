using System.Text.RegularExpressions;

namespace CryptoWatcher.ValueObjects;

public sealed partial class WalletAddress : IEquatable<WalletAddress>
{
    private static readonly Regex AddressRegex = MyRegex();

    private WalletAddress(string value) => Value = value;

    public string Value { get; }
    
    public static WalletAddress Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Address cannot be empty.", nameof(value));
        }

        return AddressRegex.IsMatch(value)
            ? new WalletAddress(value)
            : throw new ArgumentException($"Invalid Ethereum address format: {value}", nameof(value));
    }

    public bool Equals(WalletAddress? other)
        => other is not null && string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj) => Equals(obj as WalletAddress);
    public override int GetHashCode() => Value.ToLowerInvariant().GetHashCode();
    public override string ToString() => Value;

    public static implicit operator string(WalletAddress address) => address.Value;
    public static explicit operator WalletAddress(string value) => Create(value);

    [GeneratedRegex("^0x[a-fA-F0-9]{40}$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}