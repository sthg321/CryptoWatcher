using System.Diagnostics.CodeAnalysis;

namespace UniswapClient.UniswapV4.PositionsFetcher;

internal static class UniswapV4PositionFetcherAbi
{
    [StringSyntax(StringSyntaxAttribute.Json)]
    public const string Abi = """
                              [
                              {
                                "inputs": [
                                  { "internalType": "uint256", "name": "tokenId", "type": "uint256" }
                                ],
                                "name": "getPositionLiquidity",
                                "outputs": [
                                  { "internalType": "uint128", "name": "liquidity", "type": "uint128" }
                                ],
                                "stateMutability": "view",
                                "type": "function"
                              },
                              {
                                 "inputs": [
                                   { "internalType": "uint256", "name": "tokenId", "type": "uint256" }
                                 ],
                                 "name": "getPoolAndPositionInfo",
                                 "outputs": [
                                   {
                                     "components": [
                                       {
                                         "internalType": "Currency",
                                         "name": "currency0",
                                         "type": "address"
                                       },
                                       {
                                         "internalType": "Currency",
                                         "name": "currency1",
                                         "type": "address"
                                       },
                                       { "internalType": "uint24", "name": "fee", "type": "uint24" },
                                       { "internalType": "int24", "name": "tickSpacing", "type": "int24" },
                                       {
                                         "internalType": "contract IHooks",
                                         "name": "hooks",
                                         "type": "address"
                                       }
                                     ],
                                     "internalType": "struct PoolKey",
                                     "name": "poolKey",
                                     "type": "tuple"
                                   },
                                   { "internalType": "PositionInfo", "name": "info", "type": "uint256" }
                                 ],
                                 "stateMutability": "view",
                                 "type": "function"
                               },
                               {
                                 "inputs": [
                                   { "internalType": "bytes32", "name": "slot", "type": "bytes32" }
                                 ],
                                 "name": "extsload",
                                 "outputs": [{ "internalType": "bytes32", "name": "", "type": "bytes32" }],
                                 "stateMutability": "view",
                                 "type": "function"
                               },
                               {
                                "inputs": [
                                  { "internalType": "PoolId", "name": "poolId", "type": "bytes32" },
                                  { "internalType": "int24", "name": "tickLower", "type": "int24" },
                                  { "internalType": "int24", "name": "tickUpper", "type": "int24" }
                                ],
                                "name": "getFeeGrowthInside",
                                "outputs": [
                                  {
                                    "internalType": "uint256",
                                    "name": "feeGrowthInside0X128",
                                    "type": "uint256"
                                  },
                                  {
                                    "internalType": "uint256",
                                    "name": "feeGrowthInside1X128",
                                    "type": "uint256"
                                  }
                                ],
                                "stateMutability": "view",
                                "type": "function"
                              }
                               ]
                              """;
}