using CryptoWatcher.Modules.Uniswap.Application.Models.Reports;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions.Reports;

public interface IUniswapOverallReportService
{
    Task<Dictionary<Wallet, List<UniswapOverallReport>>> GetReportDataAsync(
        IReadOnlyCollection<Wallet> wallets,
        DateOnly from, DateOnly to,
        CancellationToken ct = default);
}