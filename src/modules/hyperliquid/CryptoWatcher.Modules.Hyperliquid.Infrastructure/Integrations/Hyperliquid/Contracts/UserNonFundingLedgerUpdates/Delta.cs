using System.Text.Json.Serialization;

namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Integrations.Hyperliquid.Contracts.UserNonFundingLedgerUpdates;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type", IgnoreUnrecognizedTypeDiscriminators = true)]
[JsonDerivedType(typeof(VaultDeposit), "vaultDeposit")]
[JsonDerivedType(typeof(VaultWithdraw), "vaultWithdraw")]
public record Delta;