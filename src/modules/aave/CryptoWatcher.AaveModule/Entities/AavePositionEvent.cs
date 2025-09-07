using System.Numerics;

namespace CryptoWatcher.AaveModule.Entities;

public class AavePositionEvent
{
    public Guid PositionId { get; set; }
    
    public BigInteger Amount { get; set; }

    public DateOnly Date { get; set; }
    
    public AavePositionType Type { get; set; }
}