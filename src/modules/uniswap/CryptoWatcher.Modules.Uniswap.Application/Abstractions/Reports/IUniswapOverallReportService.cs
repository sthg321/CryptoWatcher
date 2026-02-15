using CryptoWatcher.Modules.Uniswap.Application.Models.Reports;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions.Reports;

public interface IUniswapOverallReportService
{
    Task<Dictionary<EvmAddress, List<UniswapOverallReport>>> GetReportDataAsync(
        IReadOnlyCollection<Wallet> wallets,
        DateOnly from, DateOnly to,
        CancellationToken ct = default);
}