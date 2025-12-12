using System.Numerics;
using Bogus;
using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Extensions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Tests.DataSets;
using CryptoWatcher.Modules.Uniswap.Tests.Fakers;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;
using Shouldly;

namespace CryptoWatcher.Modules.Uniswap.Tests.Entities.UniswapPositionTests;

public partial class UniswapLiquidityPositionTest
{
    private readonly Faker _faker = new();

    [Fact]
    public void Calculate_position_fee_for_zero_length_period_returns_zero()
    {
        var randomStartDate = _faker.Date.FutureDateOnly();

        var position = CreatePositionWithSnapshots(randomStartDate, 0);

        var actual = position.CalculateLifetimeTotalFeeInUsd(_faker.Date.FutureDateOnly());

        actual.ShouldBe(0);
    }

    [Theory]
    [InlineData("2024.12.31")]
    [InlineData("2025.01.11")]
    public void Calculate_position_fee_for_period_without_claimed_fees(
        string feeClaimDate)
    {
        // Arrange
        var startDate =
            DateOnly.FromDateTime(new DateTime(DateOnly.Parse("2025.01.01"), new TimeOnly(), DateTimeKind.Utc));

        var position = CreatePositionWithSnapshots(startDate, 10);

        AddFeeClaimEvent(position, 0, DateOnly.Parse(feeClaimDate).ToMinDateTime());

        var expected = position.PoolPositionSnapshots.Last();

        // Act
        var actual = position.CalculateFeeForPeriod(startDate, expected.Day);

        // Assert
        actual.Value.ShouldBe(expected.FeeInUsd);
    }

    [Fact]
    public void Calculate_position_fee_with_empty_snapshot_for_start_range()
    {
        // Arrange
        var startDate =
            DateOnly.FromDateTime(new DateTime(DateOnly.Parse("2025.01.01"), new TimeOnly(), DateTimeKind.Utc));

        var position = CreatePositionWithSnapshots(startDate, 10);

        var expected = position.PoolPositionSnapshots.Last();

        // Act
        var actual = position.CalculateFeeForPeriod(startDate, expected.Day);

        // Assert
        actual.Value.ShouldBe(expected.FeeInUsd);
    }

    [Theory]
    [InlineData("2025.01.01", 200)]
    [InlineData("2025.01.02", 400)]
    [InlineData("2025.01.03", 800)]
    public void Calculate_position_fee_for_period_with_claimed_fees(
        string dateString, decimal expectedFeeWithoutClaimedFee)
    {
        // Arrange
        var testDate = DateOnly.Parse(dateString).ToMinDateTime();

        var startDate = DateOnly.Parse("2025.01.01");

        var position = new UniswapLiquidityPositionFaker(new UniswapChainConfigurationFaker().Generate()).Generate();

        var token0 = position.Token0;
        var token1 = position.Token1;

        position.AddOrUpdateSnapshot(startDate, true,
            _faker.Crypto().RandomTokenInfoWithFee(token0, 1, 100),
            _faker.Crypto().RandomTokenInfoWithFee(token1, 1, 100));

        position.AddOrUpdateSnapshot(DateOnly.Parse("2025.01.02"), true,
            _faker.Crypto().RandomTokenInfoWithFee(token0, 1, 200),
            _faker.Crypto().RandomTokenInfoWithFee(token1, 1, 200));

        position.AddOrUpdateSnapshot(DateOnly.Parse("2025.01.03"), true,
            _faker.Crypto().RandomTokenInfoWithFee(token0, 1, 400),
            _faker.Crypto().RandomTokenInfoWithFee(token1, 1, 400));

        var claimedCashFlow = AddFeeClaimEvent(position, 0, testDate);

        // Act
        var actual = position.CalculateFeeForPeriod(startDate, DateOnly.Parse(dateString));

        // Assert
        claimedCashFlow.Event.ShouldBe(CashFlowEvent.FeeClaim);
        actual.Value.ShouldBe(expectedFeeWithoutClaimedFee + claimedCashFlow.FeeInUsd);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    public void Calculate_position_fee_ignoring_deposit_and_withdrawal_events(int liquidityDelta)
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var position = CreatePositionWithSnapshots(startDate, 10);
        var snapshots = position.PoolPositionSnapshots.ToArray();

        var expected = position.PoolPositionSnapshots.Last();

        var claimFeeDate = _faker.PickRandom(snapshots).Day.ToMinDateTime();

        var claimFeeEvents = AddFeeClaimEvent(position, liquidityDelta, claimFeeDate);

        // Act
        var actual = position.CalculateFeeForPeriod(startDate, expected.Day);

        // Assert
        actual.Value.ShouldBe(expected.FeeInUsd);
        claimFeeEvents.Event.ShouldNotBe(CashFlowEvent.FeeClaim);
    }

    [Fact]
    public void
        Calculate_position_fee_for_period_with_multiple_fee_claims_in_different_days_()
    {
        // Arrange
        // Arrange
        var startDate = DateOnly.Parse("2025.01.01");
        var fromDate = DateOnly.Parse("2025.01.01");
        var toDate = DateOnly.Parse("2025.01.10");

        var position = CreatePositionWithSnapshots(startDate, 10);
        var snapshots = position.PoolPositionSnapshots.ToArray();

        // Добавляем несколько claims на разные дни в диапазоне
        var claimDates = new[]
        {
            new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc), // День 2
            new DateTime(2025, 1, 5, 0, 0, 0, DateTimeKind.Utc), // День 5
            new DateTime(2025, 1, 8, 0, 0, 0, DateTimeKind.Utc)  // День 8
        };

        var claimedCashFlows = claimDates.Select(date => AddFeeClaimEvent(position, 0, date)).ToList();

        // ИСПРАВЛЕНО: используем новую логику GetLastSnapshotBefore
        var snapshotBeforeFrom = snapshots.GetLastSnapshotBefore(fromDate); // null для 01.01
        var expectedUnclaimed = snapshots.Last().FeeInUsd - (snapshotBeforeFrom?.FeeInUsd ?? 0M);
    
        var expectedClaimed = claimedCashFlows.Sum(cf => cf.FeeInUsd);
        var expectedTotal = expectedUnclaimed + expectedClaimed;

        // Act
        var actual = position.CalculateFeeForPeriod(fromDate, toDate);

        // Assert
        actual.Value.ShouldBe(expectedTotal);
        claimedCashFlows.ShouldAllBe(cf => cf.Event == CashFlowEvent.FeeClaim);
    }

    [Fact]
    public void Calculate_position_fee_ignoring_fee_claims_outside_of_specified_period()
    {
        // Arrange
        var startDate = DateOnly.Parse("2025.01.01");
        var fromDate = DateOnly.Parse("2025.01.03");
        var toDate = DateOnly.Parse("2025.01.10");

        var position = CreatePositionWithSnapshots(startDate, 10);
        var snapshots = position.PoolPositionSnapshots.OrderBy(s => s.Day).ToArray();
        
        var outOfRangeClaimDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        AddFeeClaimEvent(position, _faker.Random.Number(1), outOfRangeClaimDate);
        
        var inRangeClaimDate = new DateTime(2025, 1, 5, 0, 0, 0, DateTimeKind.Utc);
        var inRangeClaim = AddFeeClaimEvent(position, _faker.Random.Long(1), inRangeClaimDate);

        var feeOnJan2 = snapshots.First(s => s.Day == DateOnly.Parse("2025.01.02")).FeeInUsd;
        var feeOnJan10 = snapshots.First(s => s.Day == DateOnly.Parse("2025.01.10")).FeeInUsd;
        var expectedPositionFee = feeOnJan10 - feeOnJan2; // Разница между 10.01 и 02.01
    
        var expectedTotal = expectedPositionFee + inRangeClaim.FeeInUsd;

        // Act
        var actual = position.CalculateFeeForPeriod(fromDate, toDate);

        // Assert
        actual.Value.ShouldBe(expectedTotal);
    }

    private UniswapLiquidityPosition CreatePositionWithSnapshots(DateOnly startDate, int daysCount)
    {
        var token0 = _faker.Crypto().TokenInfo();
        var token1 = _faker.Crypto().TokenInfoOtherThan(token0);

        var chain = new UniswapChainConfigurationFaker().Generate();
        var position = new UniswapLiquidityPositionFaker(chain).Generate();

        foreach (var i in Enumerable.Range(0, daysCount))
        {
            position.AddOrUpdateSnapshot(startDate.AddDays(i), true,
                _faker.Crypto().RandomTokenInfoWithFee(token0),
                _faker.Crypto().RandomTokenInfoWithFee(token1));
        }

        return position;
    }

    private UniswapLiquidityPositionCashFlow AddFeeClaimEvent(UniswapLiquidityPosition position,
        BigInteger liquidity,
        DateTime claimDate,
        TokenInfoPair? tokenPair = null)
    {
        var positionEvent = new LiquidityPoolPositionEventFaker(position, liquidity, claimDate).Generate();

        tokenPair ??= new TokenInfoPair
        {
            Token0 = new TokenInfoWithAddress(position.Token0, _faker.Crypto().EvmAddress()),
            Token1 = new TokenInfoWithAddress(position.Token1, _faker.Crypto().EvmAddress()),
        };

        return position.AddCashFlow(positionEvent, tokenPair);
    }
}