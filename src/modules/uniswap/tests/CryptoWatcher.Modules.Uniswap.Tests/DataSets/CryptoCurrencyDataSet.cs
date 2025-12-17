using Bogus;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Tests.DataSets;

public static class TheCryptoData
{
    public static readonly List<TokenRecord> Tokens =
    [
        new("BTC", "Bitcoin", 8),
        new("ETH", "Ethereum", 18),
        new("USDT", "Tether", 6),
        new("USDC", "USD Coin", 6),
        new("BNB", "BNB", 18),
        new("XRP", "XRP", 6),
        new("SOL", "Solana", 9),
        new("ADA", "Cardano", 6),
        new("DOGE", "Dogecoin", 8),
        new("TRX", "TRON", 6),
        new("LINK", "Chainlink", 18),
        new("MATIC", "Polygon", 18),
        new("DOT", "Polkadot", 10),
        new("SHIB", "Shiba Inu", 18),
        new("AVAX", "Avalanche", 18),
        new("WBTC", "Wrapped Bitcoin", 8),
        new("LTC", "Litecoin", 8),
        new("BCH", "Bitcoin Cash", 8),
        new("NEAR", "NEAR Protocol", 24),
        new("APT", "Aptos", 8)
    ];

    public static readonly List<NetworkRecord> Networks =
    [
        new("Unichain", new Uri("https://lb.drpc.live/unichain"), new Uri("https://unichain.blockscout.com")),
        new("Arbitrum", new Uri("https://lb.drpc.live/unichain"), new Uri("https://arbitrum.blockscout.com")),
        new("Celo", new Uri("https://lb.drpc.live/celo"), new Uri("https://celo.blockscout.com")),
        new("Celo", new Uri("https://lb.drpc.live/avalanche"), new Uri("https://avalanche.blockscout.com")),
        new("Celo", new Uri("https://lb.drpc.live/ethereum"), new Uri("https://ethereum.blockscout.com"))
    ];

    public record TokenRecord(string Symbol, string Name, int Decimals);

    public record NetworkRecord(string Name, Uri Rpc, Uri Blockscout);
}

public class Crypto : DataSet
{
    private int _evmAddressCounter = 0;

    public CryptoToken TokenInfo()
    {
        var token = PickToken();
        var amount = Random.Decimal(0.00000001m, 10_000m);
        amount = Math.Round(amount, token.Decimals, MidpointRounding.AwayFromZero);

        return new CryptoToken
        {
            Symbol = token.Symbol,
            Amount = amount,
            PriceInUsd = Random.Decimal(0.0001m, 200_000m)
        };
    }

    public TokenInfoWithFee RandomTokenInfoWithFee(CryptoToken cryptoToken)
    {
        return TokenInfoWithFee.Create(cryptoToken, Random.Decimal(0.0001m, 200_000m), Random.Decimal(0, 20000));
    }

    public TokenInfoWithFee RandomTokenInfoWithFee(CryptoToken cryptoToken, decimal amount, decimal priceInUsd)
    {
        return TokenInfoWithFee.Create(cryptoToken, amount, priceInUsd);
    }

    public TokenInfoWithFee RandomTokenInfoWithFee()
    {
        var token = TokenInfo();
        return TokenInfoWithFee.Create(token, Random.Decimal(0.0001m, 200_000m), Random.Decimal(0, 20000));
    }

    public CryptoToken RandomTokenInfoWithAddress()
    {
        var token = PickToken();

        return new CryptoToken
        {
            Symbol = token.Symbol,
            Amount = Random.Decimal(0.00000001m, 10_000m),
            PriceInUsd = Random.Decimal(0.0001m, 200_000m),
            Address = EvmAddress()
        };
    }

    public CryptoToken RandomTokenInfoWithAddressOtherThan(CryptoToken cryptoToken)
    {
        var address = EvmAddress();

        while (address == cryptoToken.Address)
        {
            address = EvmAddress();
        }

        var token = PickToken();

        return new CryptoToken
        {
            Symbol = token.Symbol,
            Amount = Random.Decimal(0.00000001m, 10_000m),
            PriceInUsd = Random.Decimal(0.0001m, 200_000m),
            Address = address
        };
    }

    public TokenInfoWithFee RandomTokenInfoWithFeeOtherThan(CryptoToken cryptoToken)
    {
        var address = EvmAddress();

        while (address == cryptoToken.Address)
        {
            address = EvmAddress();
        }

        return TokenInfoWithFee.Create(cryptoToken, Random.Decimal(0.0001m, 200_000m), Random.Decimal(0, 20000));
    }

    public CryptoToken TokenInfoOtherThan(CryptoToken previous)
    {
        var available = TheCryptoData.Tokens
            .Where(t => t.Symbol != previous.Symbol)
            .ToList();

        if (available.Count == 0)
        {
            throw new InvalidOperationException("There is no other tokens.");
        }

        var token = Random.ListItem(available);

        var amount = Random.Decimal(0.0001m, 10_000m);
        amount = Math.Round(amount, token.Decimals, MidpointRounding.AwayFromZero);

        return new CryptoToken
        {
            Symbol = token.Symbol,
            Amount = amount,
            PriceInUsd = Random.Decimal(0.0001m, 200_000m)
        };
    }

    public TheCryptoData.NetworkRecord Network() => Random.ListItem(TheCryptoData.Networks);

    public EvmAddress EvmAddress() =>
        CryptoWatcher.ValueObjects.EvmAddress.Create($"0x{_evmAddressCounter++.ToString("X").PadLeft(40, '0')}");

    public string TxHash() => "0x" + Random.Hash(64).ToLower();

    private TheCryptoData.TokenRecord PickToken() =>
        Random.ListItem(TheCryptoData.Tokens);
}