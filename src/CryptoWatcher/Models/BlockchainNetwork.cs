namespace CryptoWatcher.Models;

public record BlockchainNetwork
{
    public int Id { get; init; }
    
    public string Name { get; init; } = null!;
}