namespace CryptoWatcher.Modules.Morpho.Infrastructure.MorphoApiClient.Contracts;

public static class UserByAddressQuery
{
    public const string Query = """
                                        query UserByAddress($address: String!, $chainId: Int) {
                                          userByAddress(address: $address, chainId: $chainId) {
                                            marketPositions {
                                              id
                                              healthFactor
                                              market {
                                                id
                                                collateralAsset {
                                                  address
                                                  decimals
                                                  name
                                                  oraclePriceUsd
                                                  symbol
                                                  priceUsd
                                                }
                                              }
                                              state {
                                                borrowAssets
                                                borrowAssetsUsd
                                                borrowPnl
                                                borrowPnlUsd
                                                collateral
                                                collateralPnlUsd
                                                collateralRoeUsd
                                              }
                                            }
                                          }
                                        }
                                """;
}