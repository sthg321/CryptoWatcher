namespace CryptoWatcher.Modules.Fluid.Entities.Supply;

public class FluidSupplyPositionPeriod
{
    public Guid Id { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? ClosedAt { get; set; }
}