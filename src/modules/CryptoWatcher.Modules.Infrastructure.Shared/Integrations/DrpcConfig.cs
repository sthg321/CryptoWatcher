namespace CryptoWatcher.Modules.Infrastructure.Shared.Integrations;

public class DrpcConfig
{
    public Uri Host { get; set; } = null!;
    
    public string Token { get; set; } = null!;

    public int SegmentPerWindow { get; set; } = 20;

    public int PermitLimit { get; set; } = 20;

    public int QueueLimit { get; set; } = int.MaxValue;
    
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);


}