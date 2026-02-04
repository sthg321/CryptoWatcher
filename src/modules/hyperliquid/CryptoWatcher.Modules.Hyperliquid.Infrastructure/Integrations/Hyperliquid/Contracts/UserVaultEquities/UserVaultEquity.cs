namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Integrations.Hyperliquid.Contracts.UserVaultEquities;

/// <summary>
/// Represents a vault equity.
/// </summary>
/// <param name="VaultAddress">The vault address</param>
/// <param name="Equity">Equity in the vault in usd</param>
/// <param name="LockedUntilTimestamp">Locked timestamp in milliseconds</param>
public record UserVaultEquity(string VaultAddress, string Equity, long LockedUntilTimestamp);