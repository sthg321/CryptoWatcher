using System.Text.Json.Serialization;

namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client.UserNonFundingLedgerUpdates.Contracts;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type", IgnoreUnrecognizedTypeDiscriminators = true)]
[JsonDerivedType(typeof(VaultDeposit), "vaultDeposit")]
[JsonDerivedType(typeof(VaultWithdraw), "vaultWithdraw")]
public record Delta(string Type);