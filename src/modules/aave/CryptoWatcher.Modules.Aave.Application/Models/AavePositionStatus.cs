using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Application.Models;

public class AavePositionStatus
{
    public CryptoToken Token { get; set; } = null!;

    public AavePositionType PositionType { get; set; }
}