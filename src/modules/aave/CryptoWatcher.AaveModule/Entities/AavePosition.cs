using System.Numerics;
using CryptoWatcher.AaveModule.Models;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.Shared.ValueObjects;
using JetBrains.Annotations;

namespace CryptoWatcher.AaveModule.Entities;

/// <summary>
/// Represents a position in the Aave protocol that tracks supply or borrow activities for a specific wallet
/// on a given network.
/// </summary>
/// <remarks>
/// This class maintains the lifecycle of a position, including its creation and optional closure dates,
/// along with snapshots of token-specific metrics over time.
/// </remarks>
public class AavePosition
{
    [UsedImplicitly] // for ef core
    private AavePosition()
    {
    }

    public AavePosition(AaveNetwork network, Wallet wallet, AavePositionType positionType, string tokenAddress)
    {
        Network = network.Name;
        WalletAddress = wallet.Address;
        PositionType = positionType;
        TokenAddress = tokenAddress;
        CreatedAtDay = DateOnly.FromDateTime(DateTime.UtcNow);
        Id = GeneratePositionId();
    }

    /// <summary>
    /// Represents the unique identifier for the Aave position.
    /// </summary>
    /// <remarks>
    /// This property is a globally unique value used to identify and distinguish
    /// one Aave position record from another within the system.
    /// It is typically derived from specific details of the position, such as
    /// the network, wallet address, token information, and position type.
    /// </remarks>
    public Guid Id { get; private set; }

    /// <summary>
    /// Specifies the network on which the Aave position exists.
    /// </summary>
    /// <remarks>
    /// This property indicates the blockchain or layer-2 protocol where the Aave position is maintained.
    /// It helps differentiate positions across different networks, such as Ethereum, Celo, or other supported chains.
    /// </remarks>
    public string Network { get; private set; } = null!;

    /// <summary>
    /// Specifies the type of position in the Aave protocol, such as whether it represents a supply or borrow activity.
    /// </summary>
    /// <remarks>
    /// This property is used to indicate and distinguish the nature of the position,
    /// whether the wallet has supplied or borrowed tokens within the Aave ecosystem.
    /// It plays a critical role in classifying and managing position-related data.
    /// </remarks>
    public AavePositionType PositionType { get; private set; }

    /// <summary>
    /// Represents the day on which the Aave position was created.
    /// </summary>
    /// <remarks>
    /// This property records the creation date of the position in the Aave protocol. It is used to
    /// track when the position was initially established on the specified network for the associated wallet.
    /// </remarks>
    public DateOnly CreatedAtDay { get; private set; }

    /// <summary>
    /// Represents the day on which the Aave position was closed, if applicable.
    /// </summary>
    /// <remarks>
    /// This property stores the closure date of the position in the Aave protocol. It will have a value
    /// if the position has been closed; otherwise, it remains null, indicating the position is still active.
    /// </remarks>
    public DateOnly? ClosedAtDay { get; private set; }

    /// <summary>
    /// Represents the wallet address associated with the liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property holds the blockchain wallet address linked to the liquidity pool position.
    /// It is used to identify the owner of the position and manage the related account details.
    /// </remarks>
    public string WalletAddress { get; private set; } = null!;

    /// <summary>
    /// Represents the wallet associated with a liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property identifies the wallet that holds ownership of the liquidity pool position.
    /// It includes the wallet's unique identifier and blockchain address for managing assets.
    /// </remarks>
    public Wallet Wallet { get; private set; } = null!;

    /// <summary>
    /// Specifies the address of the token associated with the Aave position.
    /// </summary>
    /// <remarks>
    /// This property identifies the specific token being borrowed or supplied in the position.
    /// The token's address acts as a unique identifier on the blockchain, enabling precise tracking
    /// of assets involved in the position.
    /// </remarks>
    public string TokenAddress { get; private set; } = null!;

    // public BigInteger? PreviousScaledATokenBalance { get; set; }
    //
    // public BigInteger? PreviousScaledVariableDebt { get; set; }
    
    /// <summary>
    /// Holds a collection of snapshots representing the state of a position over time.
    /// </summary>
    /// <remarks>
    /// Each snapshot captures token-specific metrics, such as balance or interactions, for a particular day.
    /// This property provides a historical view of the position's evolution in the Aave protocol.
    /// </remarks>
    public List<AavePositionSnapshot> PositionSnapshots { get; private set; } = [];
 
    /// <summary>
    /// 
    /// </summary>
    /// <param name="day"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void ClosePosition(DateOnly day)
    {
        if (ClosedAtDay.HasValue)
        {
            throw new InvalidOperationException("Position is already closed");
        }
        
        ClosedAtDay = day;
    }

    /// <summary>
    /// Adds or updates a snapshot for the current position based on the provided token information and date.
    /// </summary>
    /// <param name="token">The token information containing details such as symbol, amount, and price in USD.</param>
    /// <param name="day">The date for which the snapshot is to be added or updated.</param>
    public void AddOrUpdateSnapshot(TokenInfo token, DateOnly day)
    {
        if (ClosedAtDay.HasValue)
        {
            throw new InvalidOperationException("Snapshot can't be added to closed position");
        }
        
        var existingSnapshot = PositionSnapshots.FirstOrDefault(s => s.Day == day);
        if (existingSnapshot != null)
        {
            existingSnapshot.SetToken(token);
            return;
        }
        
        PositionSnapshots.Add(new AavePositionSnapshot(Id, day, token));
    }

    private Guid GeneratePositionId()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Network);
        ArgumentException.ThrowIfNullOrWhiteSpace(WalletAddress);
        ArgumentException.ThrowIfNullOrWhiteSpace(TokenAddress);

        var keyData = $"{Network}:{WalletAddress}:{TokenAddress}:{PositionType}";
        var hash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(keyData));

        return new Guid(hash.Take(16).ToArray());
    }
}