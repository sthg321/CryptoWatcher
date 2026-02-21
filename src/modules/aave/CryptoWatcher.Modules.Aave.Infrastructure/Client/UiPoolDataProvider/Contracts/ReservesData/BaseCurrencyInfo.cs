using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CryptoWatcher.Modules.Aave.Infrastructure.Client.UiPoolDataProvider.Contracts.ReservesData;

[FunctionOutput]
public class BaseCurrencyInfo
{
    [Parameter("uint256", "marketReferenceCurrencyUnit", 1)]
    public BigInteger MarketReferenceCurrencyUnit { get; set; }

    [Parameter("int256", "marketReferenceCurrencyPriceInUsd", 2)]
    public BigInteger MarketReferenceCurrencyPriceInUsd { get; set; }

    [Parameter("int256", "networkBaseTokenPriceInUsd", 3)]
    public BigInteger NetworkBaseTokenPriceInUsd { get; set; }

    [Parameter("uint8", "networkBaseTokenPriceDecimals", 4)]
    public BigInteger NetworkBaseTokenPriceDecimals { get; set; }
}