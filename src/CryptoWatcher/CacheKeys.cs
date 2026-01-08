namespace CryptoWatcher;

public static class CacheKeys
{
    public static class TokenPrice
    {
        public const string TokenPriceInUsdByTokenSymbolCacheKeyTemplate = "token:{0}:tokenPriceInUsd";
        public const string TokenPriceInUsdByAddressCacheKeyTemplate = "token:{0}:tokenPriceInUsd";
        public const int CacheLifetimeInSecond = 3600;
    }
    
    public static class TokenDecimals
    {
        public const string TokenDecimalsByTokenAddressTemplate = "token:tokenAddress:{0}:tokenDecimals";
        public const int CacheLifetimeInSecond = int.MaxValue;
    }
    
    public static class TokenSymbol
    {
        public const string TokenSymbolByTokenAddressTemplate = "token:tokenAddress:{0}:tokenSymbol";
        public const int CacheLifetimeInSecond = int.MaxValue;
    }
    
    public static class TokenId
    {
        public const string TokenIdByTokenSymbolTemplate = "token:tokenSybmol:{0}:tokenId";
        public const int CacheLifetimeInSecond = int.MaxValue;
    }
    
}