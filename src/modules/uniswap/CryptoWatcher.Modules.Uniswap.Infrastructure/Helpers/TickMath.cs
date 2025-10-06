using System.Globalization;
using System.Numerics;

namespace CryptoWatcher.UniswapModule.Helpers;

/// <summary>
/// Tick math prot from uniswap TickMath.sol
/// </summary>
internal static class TickMath
{
    private const int MinTick = -887272;
    private const int MaxTick = 887272;

    private static readonly BigInteger[] RatioS =
    [
        ParseHex("fffcb933bd6fad37aa2d162d1a594001"),
        ParseHex("fff97272373d413259a46990580e213a"),
        ParseHex("fff2e50f5f656932ef12357cf3c7fdcc"),
        ParseHex("ffe5caca7e10e4e61c3624eaa0941cd0"),
        ParseHex("ffcb9843d60f6159c9db58835c926644"),
        ParseHex("ff973b41fa98c081472e6896dfb254c0"),
        ParseHex("ff2ea16466c96a3843ec78b326b52861"),
        ParseHex("fe5dee046a99a2a811c461f1969c3053"),
        ParseHex("fcbe86c7900a88aedcffc83b479aa3a4"),
        ParseHex("f987a7253ac413176f2b074cf7815e54"),
        ParseHex("f3392b0822b70005940c7a398e4b70f3"),
        ParseHex("e7159475a2c29b7443b29c7fa6e889d9"),
        ParseHex("d097f3bdfd2022b8845ad8f792aa5825"),
        ParseHex("a9f746462d870fdf8a65dc1f90e061e5"),
        ParseHex("70d869a156d2a1b890bb3df62baf32f7"),
        ParseHex("31be135f97d08fd981231505542fcfa6"),
        ParseHex("9aa508b5b7a84e1c677de54f3e99bc9"),
        ParseHex("5d6af8dedb81196699c329225ee604"),
        ParseHex("2216e584f5fa1ea926041bedfe98"),
        ParseHex("48a170391f7dc42444e8fa2")
    ];

    /// <summary>
    /// Calculates and returns the square root ratio at the specified tick.
    /// </summary>
    /// <param name="tick">
    /// The tick value for which the square root ratio is to be calculated.
    /// </param>
    /// <returns>
    /// The square root ratio as a <see cref="BigInteger"/> for the provided tick.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the tick value is outside the allowed range of ticks (-887272 to 887272).
    /// </exception>
    public static BigInteger GetSqrtRatioAtTick(int tick)
    {
        if (tick is < MinTick or > MaxTick)
            throw new ArgumentOutOfRangeException(nameof(tick));

        var absTick = BigInteger.Abs(tick);
        var ratio = (absTick & 1) != 0
            ? ParseHex("fffcb933bd6fad37aa2d162d1a594001")
            : ParseHex("100000000000000000000000000000000");

        for (var i = 1; i < RatioS.Length; i++)
        {
            if ((absTick & (BigInteger.One << i)) != 0)
                ratio = (ratio * RatioS[i]) >> 128;
        }

        if (tick > 0)
            ratio = BigInteger.Divide(BigInteger.One << 256, ratio);

        // Round to Q96
        return (ratio >> 32) + ((ratio & 0xFFFFFFFF) > 0 ? 1 : 0);
    }

    private static BigInteger ParseHex(string hex)
    {
        hex = hex.StartsWith("0x") ? hex[2..] : hex;
        if (hex.Length % 2 != 0)
        {
            hex = "0" + hex;
        }

        var bytes = new byte[hex.Length / 2 + 1];
        for (var i = 0; i < bytes.Length - 1; i++)
        {
            var byteStr = hex.Substring(hex.Length - 2 - i * 2, 2);
            bytes[i] = byte.Parse(byteStr, NumberStyles.HexNumber);
        }

        return new BigInteger(bytes);
    }
}