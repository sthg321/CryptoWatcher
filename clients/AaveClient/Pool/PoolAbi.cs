using System.Diagnostics.CodeAnalysis;

namespace AaveClient.Pool;

public class PoolAbi
{
    [StringSyntax(StringSyntaxAttribute.Json)]
    public const string Abi = """
                              [
                              {
                                "inputs": [
                                  { "internalType": "address", "name": "asset", "type": "address" }
                                ],
                                "name": "getReserveData",
                                "outputs": [
                                  {
                                    "components": [
                                      {
                                        "components": [
                                          { "internalType": "uint256", "name": "data", "type": "uint256" }
                                        ],
                                        "internalType": "struct DataTypes.ReserveConfigurationMap",
                                        "name": "configuration",
                                        "type": "tuple"
                                      },
                                      {
                                        "internalType": "uint128",
                                        "name": "liquidityIndex",
                                        "type": "uint128"
                                      },
                                      {
                                        "internalType": "uint128",
                                        "name": "currentLiquidityRate",
                                        "type": "uint128"
                                      },
                                      {
                                        "internalType": "uint128",
                                        "name": "variableBorrowIndex",
                                        "type": "uint128"
                                      },
                                      {
                                        "internalType": "uint128",
                                        "name": "currentVariableBorrowRate",
                                        "type": "uint128"
                                      },
                                      {
                                        "internalType": "uint128",
                                        "name": "currentStableBorrowRate",
                                        "type": "uint128"
                                      },
                                      {
                                        "internalType": "uint40",
                                        "name": "lastUpdateTimestamp",
                                        "type": "uint40"
                                      },
                                      { "internalType": "uint16", "name": "id", "type": "uint16" },
                                      {
                                        "internalType": "address",
                                        "name": "aTokenAddress",
                                        "type": "address"
                                      },
                                      {
                                        "internalType": "address",
                                        "name": "stableDebtTokenAddress",
                                        "type": "address"
                                      },
                                      {
                                        "internalType": "address",
                                        "name": "variableDebtTokenAddress",
                                        "type": "address"
                                      },
                                      {
                                        "internalType": "address",
                                        "name": "interestRateStrategyAddress",
                                        "type": "address"
                                      },
                                      {
                                        "internalType": "uint128",
                                        "name": "accruedToTreasury",
                                        "type": "uint128"
                                      },
                                      { "internalType": "uint128", "name": "unbacked", "type": "uint128" },
                                      {
                                        "internalType": "uint128",
                                        "name": "isolationModeTotalDebt",
                                        "type": "uint128"
                                      }
                                    ],
                                    "internalType": "struct DataTypes.ReserveDataLegacy",
                                    "name": "res",
                                    "type": "tuple"
                                  }
                                ],
                                "stateMutability": "view",
                                "type": "function"
                              }
                              ]
                              """;
}