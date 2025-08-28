namespace CryptoWatcher.Entities;

public class LendingPosition
{
    //public TokenInfo Token { get; set; } = null!;
    
    public string WalletAddress { get; init; } = null!; 
    
    public Wallet Wallet { get; set; } = null!;
}