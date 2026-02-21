namespace CryptoWatcher.Modules.Morpho.Infrastructure.MorphoApiClient.Contracts;

internal static class UserByAddressQuery
{
    public const string Query = """
                                 query UserByAddress($address: String!, $chainId: Int) {
                                  userByAddress(address: $address, chainId: $chainId) {
                                    marketPositions {
                                      id
                                      healthFactor
                                      market {
                                        id
                                        lltv
                                        loanAsset {
                                          address
                                          decimals
                                          name
                                          symbol
                                          priceUsd
                                        }
                                        collateralAsset {
                                          address
                                          decimals
                                          name
                                          symbol
                                          priceUsd
                                        }
                                      }
                                      state {
                                        borrowAssets
                                        collateral
                                      }
                                    }
                                  }
                                }
                                """;
}