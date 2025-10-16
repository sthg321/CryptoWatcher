namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client.UserNonFundingLedgerUpdates.Contracts;

public record VaultDeposit(string Vault, decimal Usdc, string Type) : Delta(Type);