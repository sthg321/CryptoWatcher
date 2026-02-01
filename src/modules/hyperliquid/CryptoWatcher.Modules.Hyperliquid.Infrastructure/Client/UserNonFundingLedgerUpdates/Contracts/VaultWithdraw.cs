namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client.UserNonFundingLedgerUpdates.Contracts;

public record VaultWithdraw(string Vault, string User, decimal NetWithdrawnUsd) : Delta;