using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Merkl.Application.Abstractions;
using CryptoWatcher.Modules.Merkl.Entities;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Specifications;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Reports;

public abstract class BaseReportService<TReport> where TReport : class
{
    private readonly IRepository<UniswapLiquidityPosition> _poolPositionRepository;
    private readonly IMerklRewardService _merklRewardService;

    public BaseReportService(IRepository<UniswapLiquidityPosition> poolPositionRepository,
        IMerklRewardService merklRewardService)
    {
        _poolPositionRepository = poolPositionRepository;
        _merklRewardService = merklRewardService;
    }

    public async Task<Dictionary<EvmAddress, List<TReport>>> GetReportDataAsync(IReadOnlyCollection<Wallet> wallets,
        DateOnly from, DateOnly to,
        CancellationToken ct = default)
    {
        var poolPositions =
            await _poolPositionRepository.ListAsync(new UniswapPositionsForReportSpecification(wallets, from, to), ct);

        var result = new Dictionary<EvmAddress, List<TReport>>();

        foreach (var poolPositionByWallet in poolPositions.GroupBy(position => position.WalletAddress))
        {
            foreach (var poolPosition in poolPositionByWallet.OrderBy(position => position.PositionId)
                         .ThenBy(position => position.IsClosed))
            {
                if (poolPosition.Snapshots.Count == 0)
                {
                    continue;
                }

                var rewards = (await _merklRewardService.GetUniswapRewardsAsync(poolPositionByWallet.Key, from, to, ct))
                    .ToDictionary(reward => reward.GetUniswapId());

                var merklCampaign = rewards.GetValueOrDefault(poolPosition.PositionId);

                var report = CreateReportItem(poolPosition, merklCampaign, from, to);

                if (!result.TryGetValue(poolPosition.WalletAddress, out var dailyReports))
                {
                    result.Add(poolPosition.WalletAddress, dailyReports = []);
                }

                dailyReports.Add(report);
            }
        }

        return result;
    }

    protected abstract TReport CreateReportItem(UniswapLiquidityPosition position, MerklCampaign? merklCampaign,
        DateOnly from, DateOnly to);
}