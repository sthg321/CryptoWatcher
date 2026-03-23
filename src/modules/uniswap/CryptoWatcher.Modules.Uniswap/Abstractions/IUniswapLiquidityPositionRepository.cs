using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Uniswap.Abstractions;

public interface IUniswapLiquidityPositionRepository
{
    Task<UniswapLiquidityPosition[]> GetAllAsync(CancellationToken ct = default);

    Task<IReadOnlyCollection<UniswapLiquidityPosition>> GetActiveAsync(UniswapChainConfiguration chain,
        ulong[] positionIds,
        CancellationToken ct = default);

    Task<UniswapLiquidityPosition[]> GetForReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly from, DateOnly to,
        CancellationToken ct = default);
    
    Task SaveAsync(UniswapLiquidityPosition[] positions, CancellationToken ct = default);

    Task<UniswapLiquidityPosition[]> GetActivePositionsAsync(CancellationToken ct = default);
}