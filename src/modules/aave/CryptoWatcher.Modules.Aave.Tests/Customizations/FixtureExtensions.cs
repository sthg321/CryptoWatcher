using AutoFixture;

namespace CryptoWatcher.Modules.Aave.Tests.Customizations;

public static class FixtureExtensions
{
    public static Fixture WithTokenDecimalsRange(this Fixture fixture)
    {
        return fixture;
    }
}