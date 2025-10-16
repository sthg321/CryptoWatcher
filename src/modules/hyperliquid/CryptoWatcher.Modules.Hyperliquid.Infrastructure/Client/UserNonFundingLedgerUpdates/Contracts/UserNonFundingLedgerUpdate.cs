namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client.UserNonFundingLedgerUpdates.Contracts;

public record UserNonFundingLedgerUpdate(long Time, string Hash, Delta Delta);