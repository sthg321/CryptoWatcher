using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;

namespace CryptoWatcher.Modules.Uniswap.Application.Models;

public class WalletTransactionScanResult
{
    public UniswapPositionEvent? Event { get; set; }

    public BlockchainTransaction Transaction { get; set; } = null!;
}