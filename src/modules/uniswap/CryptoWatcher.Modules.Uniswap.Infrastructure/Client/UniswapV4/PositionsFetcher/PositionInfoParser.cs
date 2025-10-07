using System.Globalization;
using System.Numerics;

namespace UniswapClient.UniswapV4.PositionsFetcher;

internal struct PositionInfo
{
    public int TickLower;
    public int TickUpper;
}

internal static class PositionInfoParser
{
    private static readonly BigInteger MaskUpper200Bits = BigInteger.Parse(
        "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000000000000",
        NumberStyles.HexNumber);

    private const uint Mask24Bits = 0xFFFFFF;

    private const int TickLowerOffset = 8;
    private const int TickUpperOffset = 32;

    public static PositionInfo FromUInt256(BigInteger value)
    {
        // tickLower = 24 бита начиная с 8 позиции
        var tickLower = (int)((value >> TickLowerOffset) & Mask24Bits);
        // приведение знака (поскольку int24)
        if ((tickLower & (1 << 23)) != 0)
            tickLower |= unchecked((int)0xFF000000);

        // tickUpper = 24 бита начиная с 32 позиции
        var tickUpper = (int)((value >> TickUpperOffset) & Mask24Bits);
        if ((tickUpper & (1 << 23)) != 0)
            tickUpper |= unchecked((int)0xFF000000);

        // poolId = верхние 200 бит
        var poolIdBig = value & MaskUpper200Bits;
        var poolIdBytes = new byte[25];
        var poolIdFullBytes = poolIdBig.ToByteArray(isUnsigned: true, isBigEndian: true);

        // poolIdFullBytes может быть короче 32 байт (если старшие нули), дополним до 32 перед срезкой
        if (poolIdFullBytes.Length < 32)
        {
            var temp = new byte[32];
            Array.Copy(poolIdFullBytes, 0, temp, 32 - poolIdFullBytes.Length, poolIdFullBytes.Length);
            poolIdFullBytes = temp;
        }

        Array.Copy(poolIdFullBytes, 0, poolIdBytes, 0, 25);

        return new PositionInfo
        {
            TickLower = tickLower,
            TickUpper = tickUpper,
        };
    }
}