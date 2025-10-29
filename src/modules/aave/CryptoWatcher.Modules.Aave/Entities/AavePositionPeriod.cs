using CryptoWatcher.Exceptions;

namespace CryptoWatcher.Modules.Aave.Entities;

public class AavePositionPeriod
{
    //for ef
    private AavePositionPeriod()
    {
        
    }
    
    public AavePositionPeriod(Guid positionId, DateOnly startDate)
    {
        PositionId = positionId;
        StartedAtDay = startDate;
    }
    
    public Guid Id { get; private set; }
    
    public Guid PositionId { get; private set; }
    
    public DateOnly StartedAtDay { get; private set; }
    
    public DateOnly? ClosedAtDay { get; private set; }
    
    public void Close(DateOnly closedAtDay)
    {
        if (ClosedAtDay.HasValue)
        {
            throw new DomainException("Period is already closed");
        }

        if (closedAtDay < StartedAtDay)
        {
            throw new DomainException("Close date cannot be before start date");
        }

        ClosedAtDay = closedAtDay;
    }
}