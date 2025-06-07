using System.Numerics;

namespace UniswapClient.Models;

public interface IUniswapPosition
{
    /// <summary>
    /// Gets the address or symbol of the first token in the Uniswap liquidity pool.
    /// </summary>
    string Token0 { get; }

    /// <summary>
    /// Gets the address or symbol of the second token in the Uniswap liquidity pool.
    /// </summary>
    string Token1 { get; }

    /// <summary>
    /// Gets the lower tick boundary of the Uniswap liquidity position.
    /// </summary>
    int TickLower { get; }

    /// <summary>
    /// Represents the upper tick of an Uniswap liquidity position, delimiting the price range
    /// at which the position is active within the liquidity pool.
    /// </summary>
    int TickUpper { get; }

    /// <summary>
    /// Gets the amount of liquidity provided in the Uniswap pool for the specified position.
    /// </summary>
    BigInteger Liquidity { get; }

    /// <summary>
    /// Gets the fee growth of token0 inside the tick range of the position, represented as a fixed point number with 128 decimals.
    /// This value reflects the accumulated fees earned by token0 since the last update specifically for the position's tick range.
    /// </summary>
    BigInteger FeeGrowthInside0LastX128 { get; }

    /// <summary>
    /// Gets the total fee growth for the second token within the tick range of the position,
    /// as accumulated up to the last time the position was updated, expressed in Q128 format.
    /// </summary>
    BigInteger FeeGrowthInside1LastX128 { get; }

    /// <summary>
    /// Gets the identifier for the liquidity position within the Uniswap pool.
    /// </summary>
    BigInteger PositionId { get; }
    
    /// <summary>
    /// Protocol version
    /// </summary>
    int ProtocolVersion { get; }
}