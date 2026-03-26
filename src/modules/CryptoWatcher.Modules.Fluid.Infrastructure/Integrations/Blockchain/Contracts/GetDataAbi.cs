using System.Diagnostics.CodeAnalysis;

namespace CryptoWatcher.Modules.Fluid.Infrastructure.Integrations.Blockchain.Contracts;

public class GetDataAbi
{
  [StringSyntax(nameof(StringSyntaxAttribute.Json))]
    public const string Abi = """
                              [{
                                "inputs": [],
                                "name": "getData",
                                "outputs": [
                                  {
                                    "internalType": "contract IFluidLiquidity",
                                    "name": "liquidity_",
                                    "type": "address"
                                  },
                                  {
                                    "internalType": "contract IFluidLendingFactory",
                                    "name": "lendingFactory_",
                                    "type": "address"
                                  },
                                  {
                                    "internalType": "contract IFluidLendingRewardsRateModel",
                                    "name": "lendingRewardsRateModel_",
                                    "type": "address"
                                  },
                                  {
                                    "internalType": "contract IAllowanceTransfer",
                                    "name": "permit2_",
                                    "type": "address"
                                  },
                                  { "internalType": "address", "name": "rebalancer_", "type": "address" },
                                  { "internalType": "bool", "name": "rewardsActive_", "type": "bool" },
                                  {
                                    "internalType": "uint256",
                                    "name": "liquidityBalance_",
                                    "type": "uint256"
                                  },
                                  {
                                    "internalType": "uint256",
                                    "name": "liquidityExchangePrice_",
                                    "type": "uint256"
                                  },
                                  {
                                    "internalType": "uint256",
                                    "name": "tokenExchangePrice_",
                                    "type": "uint256"
                                  }
                                ],
                                "stateMutability": "view",
                                "type": "function"
                              }]
                              """;
}