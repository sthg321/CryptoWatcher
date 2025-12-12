using Bogus;
using CryptoWatcher.Shared.ValueObjects;
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

public partial class Crypto : DataSet
{
    private int _evmAddressCounter = 0;

    public TokenInfo TokenInfo()
    {
        var token = PickToken();
        var amount = Random.Decimal(0.00000001m, 10_000m);
        amount = Math.Round(amount, token.Decimals, MidpointRounding.AwayFromZero);

        return new TokenInfo
        {
            Symbol = token.Symbol,
            Amount = amount,
            PriceInUsd = Random.Decimal(0.0001m, 200_000m)
        };
    }

    public TokenInfoWithFee RandomTokenInfoWithFee(TokenInfo token)
    {
        return TokenInfoWithFee.Create(token, Random.Decimal(0.0001m, 200_000m), Random.Decimal(0, 20000));
    }
    
    public TokenInfoWithFee RandomTokenInfoWithFee(TokenInfo token, decimal amount, decimal priceInUsd)
    {
        return TokenInfoWithFee.Create(token, amount, priceInUsd);
    }

    public TokenInfoWithFee RandomTokenInfoWithAddress()
    {
        var token = TokenInfo();
        return TokenInfoWithFee.Create(token, Random.Decimal(0.0001m, 200_000m), Random.Decimal(0, 20000));
    }

    public TokenInfoWithFee RandomTokenInfoWithAddressOtherThan(TokenInfoWithAddress token)
    {
        var address = EvmAddress();

        while (address == token.Address)
        {
            address = EvmAddress();
        }

        return TokenInfoWithFee.Create(token, Random.Decimal(0.0001m, 200_000m), Random.Decimal(0, 20000));
    }

    public TokenInfo TokenInfoOtherThan(TokenInfo previous)
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

        return new TokenInfo
        {
            Symbol = token.Symbol,
            Amount = amount,
            PriceInUsd = Random.Decimal(0.0001m, 200_000m)
        };
    }

    public TokenInfo TokenInfo(decimal amount)
    {
        var token = PickToken();
        var rounded = Math.Round(amount, token.Decimals, MidpointRounding.AwayFromZero);

        return new TokenInfo
        {
            Symbol = token.Symbol,
            Amount = rounded,
            PriceInUsd = Random.Decimal(0.0001m, 200_000m)
        };
    }

    public (string Symbol, string Name, int Decimals) TokenInfoTuple()
    {
        var t = PickToken();
        return (t.Symbol, t.Name, t.Decimals);
    }

    public TheCryptoData.NetworkRecord Network() => Random.ListItem(TheCryptoData.Networks);

    public string TokenSymbol() => PickToken().Symbol;

    public string TokenName() => PickToken().Name;

    public int TokenDecimals() => PickToken().Decimals;

    public CryptoWatcher.ValueObjects.EvmAddress EvmAddress() =>
        CryptoWatcher.ValueObjects.EvmAddress.Create($"0x{_evmAddressCounter++.ToString("X").PadLeft(40, '0')}");

    public string TxHash() => "0x" + Random.Hash(64).ToLower();

    private TheCryptoData.TokenRecord PickToken() =>
        Random.ListItem(TheCryptoData.Tokens);
}