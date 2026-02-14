using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace CryptoWatcher.Modules.Morpho.Infrastructure.MorphoApiClient.Contracts;

[PublicAPI]
internal record UserByAddressResponse
{
    public UserByAddress UserByAddress { get; set; } = null!;
}

[PublicAPI]
internal record UserByAddress
{
    public List<MarketPosition> MarketPositions { get; set; } = [];
}

[PublicAPI]
internal record MarketPosition
{
    public string Id { get; set; } = null!;
    public double? HealthFactor { get; set; }
    public Market Market { get; set; } = null!;
    public PositionState State { get; set; } = null!;
}

[PublicAPI]
internal record Market
{
    public Guid Id { get; set; }

    public Asset LoanAsset { get; set; } = null!;

    public Asset CollateralAsset { get; set; } = null!;
}

[PublicAPI]
internal record Asset
{
    public string Address { get; set; } = null!;

    public int Decimals { get; set; }

    public string Name { get; set; } = null!;

    public string Symbol { get; set; } = null!;

    public decimal PriceUsd { get; set; }
}

[PublicAPI]
internal record PositionState
{
    public long BorrowAssets { get; set; }

    [JsonConverter(typeof(BigIntegerConverter))]
    public BigInteger Collateral { get; set; }
}

public class BigIntegerConverter : JsonConverter<BigInteger>
{
    public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return reader.TokenType switch
        {
            JsonTokenType.String => BigInteger.Parse(reader.GetString()!),
            JsonTokenType.Number => BigInteger.Parse(reader.GetInt64().ToString()),
            _ => throw new ArgumentOutOfRangeException(nameof(reader), reader.TokenType, null)
        };
    }

    public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}