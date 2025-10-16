using System.Numerics;
using System.Security.Cryptography;
using AutoFixture;
using AutoFixture.Kernel;

namespace CryptoWatcher.AaveModule.Tests.Customizations;

public class PositiveBigIntegerCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(new PositiveBigIntegerGenerator());
    }

    private class PositiveBigIntegerGenerator : ISpecimenBuilder
    {
        private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        public object Create(object request, ISpecimenContext context)
        {
            if (request is not Type type || type != typeof(BigInteger))
                return new NoSpecimen();

            return RandomBalance();
        }

        private BigInteger RandomBalance()
            => NextBigInteger(BigInteger.One, BigInteger.Pow(10, 25)); // до 1e25 in WAD

        private BigInteger NextBigInteger(BigInteger min, BigInteger max)
        {
            if (min >= max) throw new ArgumentException("min must be less than max");

            var diff = max - min;
            var bytes = diff.ToByteArray();
            BigInteger result;

            do
            {
                _rng.GetBytes(bytes);
                bytes[^1] &= 0x7F;
                result = new BigInteger(bytes);
            } while (result <= 0 || result >= diff);

            return min + result;
        }
    }
}