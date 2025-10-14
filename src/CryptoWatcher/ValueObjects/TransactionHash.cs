using System.Text.RegularExpressions;

namespace CryptoWatcher.ValueObjects;

public partial class TransactionHash : IEqualityComparer<TransactionHash>
{
    private const int TransactionHashLength = 66;

    [GeneratedRegex("^0x[a-fA-F0-9]{64}$", RegexOptions.None)]
    private static partial Regex HashValidator();

    private TransactionHash(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !IsValid(value))
        {
            throw new ArgumentException(
                $"Invalid TransactionHash: {value}. Must be 0x + 64 hex chars ({TransactionHashLength} total).");
        }

        Value = value.ToLowerInvariant();
    }

    public string Value { get;  }  

    public static TransactionHash FromString(string value) => new(value);

    public static implicit operator string(TransactionHash th) => th.Value;

    public static implicit operator TransactionHash(string value) => FromString(value);

    public override string ToString() => Value;

    public bool Equals(TransactionHash? x, TransactionHash? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;

        return string.Equals(x.Value, y.Value, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(TransactionHash obj)
    {
        return obj.Value.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }
    
    private static bool IsValid(string value) =>
        HashValidator().IsMatch(value) && value.Length == TransactionHashLength;

}