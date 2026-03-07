namespace CryptoWatcher.Modules.WalletIngestion;

public class ChainRegistry
{
    public const int EthereumChainId = 1;
    
    public const int PolygonChainId = 137;
    
    public const int Arbitrum = 42161;

    public static IEnumerable<int> GetAll()
    {
        yield return Arbitrum;
        yield return EthereumChainId;
        yield return PolygonChainId;
    }
}