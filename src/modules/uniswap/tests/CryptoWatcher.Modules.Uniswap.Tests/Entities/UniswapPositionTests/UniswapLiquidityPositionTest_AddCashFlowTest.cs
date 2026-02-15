using CryptoWatcher.Exceptions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Tests.Fakers;
using Shouldly;

namespace CryptoWatcher.Modules.Uniswap.Tests.Entities.UniswapPositionTests;

public partial class UniswapLiquidityPositionTest
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void Add_cash_flow_when_none_exists_for_date(
        int liquidityDelta)
    {
        var config = new UniswapChainConfigurationFaker().Generate();
        var position = new UniswapLiquidityPositionFaker(config).Generate();

        var claimDate = _faker.Date.Future(refDate: DateTime.UtcNow);
        var cashFlow = AddFeeClaimEvent(position, liquidityDelta, claimDate);

        position.CashFlows.ShouldHaveSingleItem();

        var actual = position.CashFlows.Single();

        actual.ShouldBeSameAs(cashFlow);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void Do_not_add_cash_flow_with_duplicate_date(int liquidityDelta)
    {
        var config = new UniswapChainConfigurationFaker().Generate();
        var position = new UniswapLiquidityPositionFaker(config).Generate();

        var claimDate = _faker.Date.Future(refDate: DateTime.UtcNow);
        var firstCashFlow = AddFeeClaimEvent(position, liquidityDelta, claimDate);
        _ = AddFeeClaimEvent(position, liquidityDelta, firstCashFlow.Date);

        position.CashFlows.ShouldHaveSingleItem();

        var actual = position.CashFlows.Single();

        actual.ShouldBeSameAs(firstCashFlow);
    }

    [Fact]
    public void Do_not_add_cash_if_position_closed()
    {
        var config = new UniswapChainConfigurationFaker().Generate();
        var position = new UniswapLiquidityPositionFaker(config).Generate();

        position.ClosePosition(_faker.Date.FutureDateOnly());

        var claimDate = _faker.Date.Future(refDate: DateTime.UtcNow);

        Should.Throw<DomainException>(() => { AddFeeClaimEvent(position, 0, claimDate); },
            UniswapLiquidityPosition.PositionClosedException);
    }
}