using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CryptoWatcher.Modules.Morpho.Infrastructure.MorphoApiClient.Contracts;

internal class UserByAddressResponse
{
    public UserByAddress UserByAddress { get; set; } = null!;
}

internal class UserByAddress
{
    public List<MarketPosition> MarketPositions { get; set; } = [];
}

internal class MarketPosition
{
    public string Id { get; set; } = null!;
    public double HealthFactor { get; set; }
    public Market Market { get; set; } = null!;
    public PositionState State { get; set; } = null!;
}

internal class Market
{
    public string Id { get; set; } = null!;

    public CollateralAsset CollateralAsset { get; set; } = null!;
}

internal class CollateralAsset
{
    public string Address { get; set; } = null!;

    public int Decimals { get; set; }

    public string Name { get; set; } = null!;

    public string Symbol { get; set; } = null!;

    public double PriceUsd { get; set; }
}

internal class PositionState
{
    public long BorrowAssets { get; set; }
    public double BorrowAssetsUsd { get; set; }
    public long BorrowPnl { get; set; }
    public double BorrowPnlUsd { get; set; }

    [JsonConverter(typeof(BigIntegerConverter))]
    public BigInteger Collateral { get; set; }

    public double CollateralPnlUsd { get; set; }
    public double CollateralRoeUsd { get; set; }
}

public class BigIntegerConverter : JsonConverter<BigInteger>
{
    public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => BigInteger.Parse(reader.GetString()!),
            JsonTokenType.Number => BigInteger.Parse(reader.GetInt64().ToString()),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}