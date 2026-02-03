using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Entities;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Services;

public class PositionSyncJob
{
    private readonly IRepository<HyperliquidVaultPosition> _repository;

    public PositionSyncJob(IRepository<HyperliquidVaultPosition> repository)
    {
        _repository = repository;
    }
}