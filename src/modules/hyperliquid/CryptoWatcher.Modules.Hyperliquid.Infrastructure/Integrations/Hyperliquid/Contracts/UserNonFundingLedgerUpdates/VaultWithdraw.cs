namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Integrations.Hyperliquid.Contracts.UserNonFundingLedgerUpdates;

public record VaultWithdraw(string Vault, string User, string NetWithdrawnUsd) : Delta;