using CryptoWatcher.Exceptions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Tests.Fakers;
using JetBrains.Annotations;
using Shouldly;

namespace CryptoWatcher.Modules.Uniswap.Tests.Entities;

[TestSubject(typeof(UniswapLiquidityPosition))]
public partial class UniswapLiquidityPositionTest
{
    [Fact]
    public void Position_initialized_with_correct_fields()
    {
        var chain = new UniswapChainConfigurationFaker().Generate();
        var positionFaker = new UniswapLiquidityPositionFaker(chain).Generate();

        var createdAt = _faker.Date.FutureDateOnly();
        var actual = new UniswapLiquidityPosition(positionFaker.PositionId, positionFaker.TickLower,
            positionFaker.TickUpper, positionFaker.Token0,
            positionFaker.Token1, positionFaker.WalletAddress, chain, createdAt);

        actual.PositionId.ShouldBe(positionFaker.PositionId);
        actual.TickLower.ShouldBe(positionFaker.TickLower);
        actual.TickUpper.ShouldBe(positionFaker.TickUpper);
        actual.Token0.ShouldBeEquivalentTo(positionFaker.Token0);
        actual.Token1.ShouldBeEquivalentTo(positionFaker.Token1);
        actual.IsActive.ShouldBeTrue();
        actual.WalletAddress.ShouldBe(positionFaker.WalletAddress);
        actual.NetworkName.ShouldBe(chain.Name);
        actual.ProtocolVersion.ShouldBe(chain.ProtocolVersion);
        actual.PoolPositionSnapshots.ShouldBeEmpty();
        actual.CashFlows.ShouldBeEmpty();
    }

    [Fact]
    public void Creating_position_with_identical_tokens_throws_exception()
    {
        Should.Throw<DomainException>(() =>
        {
            var chain = new UniswapChainConfigurationFaker().Generate();
            var position = new UniswapLiquidityPositionFaker(chain).Generate();
            var createdAt = _faker.Date.FutureDateOnly();

            _ = new UniswapLiquidityPosition(position.PositionId, position.TickLower,
                position.TickUpper, position.Token0,
                position.Token0, position.WalletAddress, chain, createdAt);
        }, "For uniswap position tokens can't be the same");
    }

    [Fact]
    public void Creating_position_with_invalid_tick_range_throws_exception()
    {
        Should.Throw<DomainException>(() =>
        {
            var chain = new UniswapChainConfigurationFaker().Generate();
            var position = new UniswapLiquidityPositionFaker(chain).Generate();

            var createdAt = _faker.Date.FutureDateOnly();

            _ = new UniswapLiquidityPosition(position.PositionId, position.TickUpper,
                position.TickLower, position.Token0,
                position.Token0, position.WalletAddress, chain, createdAt);
        }, "For uniswap position tokens can't be the same");
    }
}