using CryptoWatcher.Modules.Uniswap.Tests.Fakers;
using Shouldly;

namespace CryptoWatcher.Modules.Uniswap.Tests.Entities;

public partial class UniswapLiquidityPositionTest
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void UniswapLiquidityPosition_AddCashFlowTest_WhenCashFlowWithThisDateNotExists_ShouldAddCashFlow(
        int liquidityDelta)
    {
        var config = new UniswapChainConfigurationFaker().Generate();
        var position = new UniswapLiquidityPositionFaker(config).Generate();

        var claimDate = _faker.Date.Future(refDate: DateTime.UtcNow);
        var cashFlow = AddFeeClaimEvent(position, liquidityDelta, claimDate);

        position.CashFlows.ShouldHaveSingleItem();

        var actual = position.CashFlows.Single();

        actual.ShouldBeEquivalentTo(cashFlow);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void UniswapLiquidityPosition_AddCashFlowTest_WhenCashFlowWithThisDateExists_ShouldIgnore(int liquidityDelta)
    {
        var config = new UniswapChainConfigurationFaker().Generate();
        var position = new UniswapLiquidityPositionFaker(config).Generate();

        var claimDate = _faker.Date.Future(refDate: DateTime.UtcNow);
        var firstCashFlow = AddFeeClaimEvent(position, liquidityDelta, claimDate);
        _ = AddFeeClaimEvent(position, liquidityDelta, firstCashFlow.Date);

        position.CashFlows.ShouldHaveSingleItem();

        var actual = position.CashFlows.Single();

        actual.ShouldBeEquivalentTo(firstCashFlow);
    }
}