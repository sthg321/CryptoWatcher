using System.Numerics;
using CryptoWatcher.AaveModule.Models;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.AaveModule.Abstractions;

public interface IAaveProvider
{
    Task<BigInteger> GetAssetPriceAsync(AaveNetwork aaveNetwork, string assetAddress);
    
    Task<AaveReserveData> GetLiquidityIndex(AaveNetwork aaveNetwork, string assetAddress);
    
    Task<List<AaveLendingPosition>> GetLendingPositionAsync(AaveNetwork aaveNetwork, Wallet wallet,
        CancellationToken ct = default);
}