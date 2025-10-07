using System.Diagnostics.CodeAnalysis;

namespace UniswapClient.UniswapV3.LiquidityPoolFactory;

internal static class PoolFactoryAbi
{
    [StringSyntax(StringSyntaxAttribute.Json)]
    public const string Abi = """
                              [
                                {
                                "inputs": [
                                  { "internalType": "address", "name": "tokenA", "type": "address" },
                                  { "internalType": "address", "name": "tokenB", "type": "address" }
                                ],
                                "name": "getPairPools",
                                "outputs": [
                                  {
                                    "components": [
                                      { "internalType": "address", "name": "pool", "type": "address" },
                                      { "internalType": "int24", "name": "tickSpacing", "type": "int24" }
                                    ],
                                    "internalType": "struct SyncSwapRangePoolFactoryZKSync.PoolInfo[]",
                                    "name": "",
                                    "type": "tuple[]"
                                  }
                                ],
                                "stateMutability": "view",
                                "type": "function"
                                }
                              ]
                              """;
}