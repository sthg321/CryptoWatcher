using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace UniswapClient.UniswapV3.PositionsFetcher.Contracts;

[Function("positions", typeof(PositionsOutputDTO))]
public class PositionsFunction : FunctionMessage
{
    [Parameter("uint256", "tokenId", 1)] public BigInteger TokenId { get; set; }
}