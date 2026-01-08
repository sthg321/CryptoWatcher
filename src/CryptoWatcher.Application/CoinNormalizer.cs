namespace CryptoWatcher.Application;

public class CoinNormalizer
{
    private static readonly Dictionary<string, string> SymbolToNormalizedSymbol = new()
    {
        ["USD₮0"] = "usdt"
    };

    public string NormalizeSymbol(string symbol)
    {
        return SymbolToNormalizedSymbol.GetValueOrDefault(symbol, symbol);
    }
}