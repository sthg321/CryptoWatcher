using System.Text.RegularExpressions;

namespace CryptoWatcher.ValueObjects;

public sealed partial class EvmAddress : IEquatable<EvmAddress>, IEqualityComparer<EvmAddress>
{
    private static readonly Regex AddressRegex = MyRegex();

    private EvmAddress(string value) => Value = value;

    public string Value { get; }
    
    public static EvmAddress Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Address cannot be empty.", nameof(value));
        }

        return AddressRegex.IsMatch(value)
            ? new EvmAddress(value)
            : throw new ArgumentException($"Invalid Ethereum address format: {value}", nameof(value));
    }
    
    /// <summary>
    /// Pad address to 64 characters (32 bytes).
    /// </summary>
    /// <returns></returns>
    public string ToPaddedAddress()
    {
        return "0x000000000000000000000000" +  Value[2..];
    }

    public bool Equals(EvmAddress? other)
        => other is not null && string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj) => Equals(obj as EvmAddress);
    public override int GetHashCode() => Value.ToLowerInvariant().GetHashCode();
    public override string ToString() => Value;

    public bool Equals(EvmAddress? x, EvmAddress? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;

        return string.Equals(x.Value, y.Value, StringComparison.InvariantCultureIgnoreCase);
    }

    public int GetHashCode(EvmAddress obj)
    {
        return obj.Value.GetHashCode(StringComparison.InvariantCultureIgnoreCase);
    }
    
    public static implicit operator string(EvmAddress address) => address.Value;
    public static explicit operator EvmAddress(string value) => Create(value);

    [GeneratedRegex("^0x[a-fA-F0-9]{40}$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}