using CryptoWatcher.Abstractions.PositionSnapshots;
using CryptoWatcher.Exceptions;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Hyperliquid.Entities;

/// <summary>
/// Represents a snapshot of a position in a Hyperliquid vault, including balance, day of the snapshot,
/// vault details, and associated wallet information.
/// </summary>
public class HyperliquidVaultPositionSnapshot : ITokenPositionSnapshot
{
    private HyperliquidVaultPositionSnapshot()
    {
        
    }
    
    public HyperliquidVaultPositionSnapshot(Wallet wallet, EvmAddress vaultAddress, decimal balance,
        DateOnly day)
    {
        WalletAddress = wallet.Address;
        VaultAddress = vaultAddress;
        Balance = balance;
        Day = day;
        Token0 = new CryptoTokenStatistic
        {
            Amount = balance,
            PriceInUsd = 1
        };
    }

    /// <summary>
    /// Balance in usd
    /// </summary>
    public decimal Balance { get; private set; }

    /// <summary>
    /// The day when the snapshot was taken
    /// </summary>
    public DateOnly Day { get; private set; }
    
    public CryptoTokenStatistic Token0 { get; private set; } = null !;

    /// <summary>
    /// VaultAddress address
    /// </summary>
    public EvmAddress VaultAddress { get; init; }

    /// <summary>
    /// Represents the wallet address associated with the liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property holds the blockchain wallet address linked to the liquidity pool position.
    /// It is used to identify the owner of the position and manage the related account details.
    /// </remarks>
    public EvmAddress WalletAddress { get; init; }

    public void UpdateFrom(HyperliquidVaultPositionSnapshot newSnapshot)
    {
        if (!newSnapshot.VaultAddress.Equals(newSnapshot.VaultAddress))
        {
            throw new DomainException("Vault address does not match");
        }

        if (!newSnapshot.WalletAddress.Equals(newSnapshot.WalletAddress))
        {
            throw new DomainException("Wallet address does not match");
        }

        if (Day != newSnapshot.Day)
        {
            throw new DomainException("Day does not match");
        }

        Balance = newSnapshot.Balance;
    }
}