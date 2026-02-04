namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Integrations.Hyperliquid.Contracts.UserNonFundingLedgerUpdates;

public record VaultDeposit(string Vault, string Usdc) : Delta;