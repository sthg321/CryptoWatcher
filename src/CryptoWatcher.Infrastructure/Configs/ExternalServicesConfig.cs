namespace CryptoWatcher.Infrastructure.Configs;

public class ExternalServicesConfig
{
    public Uri Uniswap { get; set; } = null!;

    public Uri CoinGecko { get; set; } = null!;
    
    public Uri Morpho { get; set; } = null!;
}