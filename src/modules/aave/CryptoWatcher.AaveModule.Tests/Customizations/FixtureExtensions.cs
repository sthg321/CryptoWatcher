using AutoFixture;
using CryptoWatcher.AaveModule.Models;

namespace CryptoWatcher.AaveModule.Tests.Customizations;

public static class FixtureExtensions
{
    public static Fixture WithTokenDecimalsRange(this Fixture fixture)
    {
        fixture.Customize<CalculatableAaveLendingPosition>(composer =>
            composer.With(x => x.TokenDecimals, () => (byte)Random.Shared.Next(2, 19)));
        
        fixture.Customize<SuppliedAaveLendingPosition>(composer =>
            composer.With(x => x.TokenDecimals, () => (byte)Random.Shared.Next(2, 19)));
        
        fixture.Customize<BorrowedAaveLendingPosition>(composer =>
            composer.With(x => x.TokenDecimals, () => (byte)Random.Shared.Next(2, 19)));

        
        return fixture;
    }
}