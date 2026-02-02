namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Integrations.Hyperliquid.Contracts.UserNonFundingLedgerUpdates;

public record VaultWithdraw(string Vault, string User, decimal NetWithdrawnUsd) : Delta;