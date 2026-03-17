namespace CryptoWatcher.Modules.WalletIngestion;

public class ChainRegistry
{
    public const int Arbitrum = 42161;
    
    public const int Base = 8453;

    public static IEnumerable<int> GetAll()
    {
        yield return Arbitrum;
        yield return Base; 
    }
}