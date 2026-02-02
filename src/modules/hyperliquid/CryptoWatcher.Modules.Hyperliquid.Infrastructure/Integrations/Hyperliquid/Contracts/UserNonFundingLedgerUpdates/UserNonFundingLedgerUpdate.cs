namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Integrations.Hyperliquid.Contracts.UserNonFundingLedgerUpdates;

public record UserNonFundingLedgerUpdate(long Time, string Hash, Delta Delta);