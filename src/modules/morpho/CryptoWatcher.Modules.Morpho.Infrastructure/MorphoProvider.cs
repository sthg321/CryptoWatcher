using System.Numerics;
using CryptoWatcher.Extensions;
using CryptoWatcher.Modules.Morpho.Application.Abstractions;
using CryptoWatcher.Modules.Morpho.Application.Models;
using CryptoWatcher.Modules.Morpho.Infrastructure.MorphoApiClient.Contracts;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Morpho.Infrastructure;

internal class MorphoProvider : IMorphoProvider
{
    private readonly IMorphoClient _morphoClient;

    public MorphoProvider(IMorphoClient morphoClient)
    {
        _morphoClient = morphoClient;
    }

    public async Task<MorphoMarketPositionData[]> GetUserMarketPositionsAsync(EvmAddress address, int chainId,
        CancellationToken ct = default)
    {
        var result = await _morphoClient.GetUserMarketPositionsAsync(address.Value, chainId, ct);

        if (result is null)
        {
            return [];
        }
        
        return result.UserByAddress.MarketPositions
            .Select(position =>
            {
                var loanToken = CreateToken(position.Market.LoanAsset, position.State.BorrowAssets);
                var collateralToken = CreateToken(position.Market.CollateralAsset, position.State.Collateral);

                return new MorphoMarketPositionData(position.Market.Id, loanToken, collateralToken,
                    position.HealthFactor ?? int.MaxValue);
            })
            .ToArray();
    }

    private static CryptoToken CreateToken(Asset asset, BigInteger amount)
    {
        return new CryptoToken
        {
            Address = EvmAddress.Create(asset.Address),
            Amount = amount.ToDecimal(asset.Decimals),
            PriceInUsd = asset.PriceUsd,
            Symbol = asset.Symbol
        };
    }
}