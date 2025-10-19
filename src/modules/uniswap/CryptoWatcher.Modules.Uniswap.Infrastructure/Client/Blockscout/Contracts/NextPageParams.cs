using System.Text.Json.Serialization;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Client.Blockscout.Contracts;

public class NextPageParams
{
    [JsonPropertyName("index")] public int Index { get; set; }

    [JsonPropertyName("value")] public string Value { get; set; } = null!;

    [JsonPropertyName("hash")] public string Hash { get; set; } = null!;

    [JsonPropertyName("block_number")] public long BlockNumber { get; set; }

    [JsonPropertyName("inserted_at")] public DateTimeOffset? InsertedAt { get; set; }

    [JsonPropertyName("items_count")] public int ItemsCount { get; set; }
}