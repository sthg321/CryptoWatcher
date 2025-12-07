using CryptoWatcher.Exceptions;
using CryptoWatcher.Modules.Uniswap.Tests.Fakers;
using Shouldly;

namespace CryptoWatcher.Modules.Uniswap.Tests.Entities.UniswapPositionTests;

public partial class UniswapLiquidityPositionTest
{
    [Fact]
    public void Position_closed_with_provided_date()
    {
        var chain = new UniswapChainConfigurationFaker().Generate();
        var position = new UniswapLiquidityPositionFaker(chain).Generate();

        var closeDate = _faker.Date.FutureDateOnly();
        position.ClosePosition(closeDate);
        
        position.ClosedAt.ShouldBe(closeDate);
        position.IsActive.ShouldBeFalse();
    }
    
    [Fact]
    public void Cannot_close_closed_position()
    {
        var chain = new UniswapChainConfigurationFaker().Generate();
        var position = new UniswapLiquidityPositionFaker(chain).Generate();

        var closeDate = _faker.Date.FutureDateOnly();
        position.ClosePosition(closeDate);

        Should.Throw<DomainException>(() => position.ClosePosition(closeDate),
            "Can't close already closed uniswap position");
    }
}