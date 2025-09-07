using System.Diagnostics.CodeAnalysis;

namespace AaveClient.AaveOracle;

internal static class AaveOracleFetcherAbi
{
    [StringSyntax(StringSyntaxAttribute.Json)]
    public const string Abi = """
                              [
                                {
                                  "inputs": [
                                    { "internalType": "address", "name": "asset", "type": "address" }
                                  ],
                                  "name": "getAssetPrice",
                                  "outputs": [{ "internalType": "uint256", "name": "", "type": "uint256" }],
                                  "stateMutability": "view",
                                  "type": "function"
                                }
                              ]
                              """;
}