using AutoFixture;
using CryptoWatcher.AaveModule.Entities;
using CryptoWatcher.AaveModule.Models;
using CryptoWatcher.AaveModule.Tests.Customizations;
using CryptoWatcher.Extensions;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.Shared.ValueObjects;
using JetBrains.Annotations;
using Moq;

namespace CryptoWatcher.AaveModule.Tests.Entities;

[TestSubject(typeof(AavePosition))]
public class AavePositionTest
{
    private static readonly Wallet TestWallet = new() { Address = Guid.CreateVersion7().ToString() };
    private static readonly DateTimeOffset TestTime = DateTimeOffset.UtcNow;
    private static readonly DateOnly TestDate = TestTime.DateTime.ToDateOnly();
    private readonly Fixture _fixture;
    private readonly Mock<TimeProvider> _timeProviderMock = new();

    public AavePositionTest()
    {
        _fixture = new Fixture();
        _fixture.Customize(new PositiveBigIntegerCustomization());

        _timeProviderMock.Setup(provider => provider.LocalTimeZone).Returns(TimeZoneInfo.Utc);

        _timeProviderMock.Setup(provider => provider.GetUtcNow()).Returns(TestTime);
    }

    [Theory]
    [InlineData(AavePositionType.Supplied)]
    [InlineData(AavePositionType.Borrowed)]
    public void AddOrUpdateSnapshotTest_WhenPositionClosed_ShouldThrowException(AavePositionType positionType)
    {
        var fixture = new Fixture();
        var position = CreatePosition(positionType);
        position.ClosePosition(DateOnly.MaxValue);

        Assert.Throws<InvalidOperationException>(() =>
            position.AddOrUpdateSnapshot(_fixture.Create<TokenInfo>(), fixture.Create<decimal>(), TestDate,
                _timeProviderMock.Object));
    }

    [Theory]
    [InlineData(AavePositionType.Borrowed)]
    [InlineData(AavePositionType.Supplied)]
    public void AddOrUpdateSnapshotTest_WhenSnapshotForDayExist_ShouldUpdateSnapshot(AavePositionType type)
    {
        var position = CreatePosition(type);
        var token = _fixture.Create<TokenInfo>();

        var existedTokenAmount = _fixture.Create<decimal>();
        position.AddOrUpdateSnapshot(token, existedTokenAmount, TestDate, _timeProviderMock.Object);

        var expectedTokenAmount = _fixture.Create<decimal>();
        var expectedToken = token with
        {
            PriceInUsd = _fixture.Create<decimal>(),
            Amount = _fixture.Create<decimal>()
        };

        position.AddOrUpdateSnapshot(expectedToken, expectedTokenAmount, TestDate, _timeProviderMock.Object);

        var actualSnapshot = position.PositionSnapshots.First();

        Assert.Single(position.PositionSnapshots);
        Assert.Equal(position.PreviousScaledAmount, expectedTokenAmount);
        Assert.Equivalent(new AavePositionSnapshot(position.Id, TestDate, expectedToken), actualSnapshot);
    }

    [Theory]
    [InlineData(AavePositionType.Borrowed)]
    [InlineData(AavePositionType.Supplied)]
    public void AddOrUpdateSnapshotTest_WhenSnapshotForDayNotExist_ShouldUpdateSnapshot(AavePositionType type)
    {
        var syncDate = DateOnly.FromDateTime(DateTime.Now);
        var position = CreatePosition(type);
        var token = _fixture.Create<TokenInfo>();

        position.AddOrUpdateSnapshot(token, 1, syncDate, _timeProviderMock.Object);

        var actualSnapshot = position.PositionSnapshots.First();
        var expectedSnapshot = new AavePositionSnapshot(position.Id, syncDate, token);

        Assert.Single(position.PositionSnapshots);
        Assert.Equivalent(expectedSnapshot, actualSnapshot);
    }

    [Theory]
    [InlineData(AavePositionType.Borrowed)]
    [InlineData(AavePositionType.Supplied)]
    public void AddOrUpdateSnapshotTest_WhenScaleIsNull_ShouldNotAddAnyEvents(AavePositionType type)
    {
        var syncDate = DateOnly.FromDateTime(DateTime.Now);
        var expectedScaledAmount = _fixture.Create<decimal>();
        var position = CreatePosition(type);
        var token = _fixture.Create<TokenInfo>();

        position.AddOrUpdateSnapshot(token, expectedScaledAmount, syncDate, _timeProviderMock.Object);

        Assert.Empty(position.PositionEvents);
    }

    [Theory]
    [InlineData(100, 200, AavePositionType.Borrowed, AavePositionEventType.Deposit)]
    [InlineData(500, 400, AavePositionType.Supplied, AavePositionEventType.Withdrawal)]
    [InlineData(100, 50, AavePositionType.Borrowed, AavePositionEventType.Withdrawal)]
    [InlineData(500, 600, AavePositionType.Supplied, AavePositionEventType.Deposit)]
    public void AddOrUpdateSnapshotTest_WhenScaleNotChange_ShouldAddSingleDepositEvent(decimal initialScaleAmount,
        decimal updatedScaleAmount,
        AavePositionType positionType,
        AavePositionEventType eventType)
    {
        var syncDate = DateOnly.FromDateTime(DateTime.Now);
        var position = CreatePosition(positionType);
        var token = _fixture.Create<TokenInfo>();

        position.AddOrUpdateSnapshot(token, initialScaleAmount, syncDate.AddDays(1), _timeProviderMock.Object);

        position.AddOrUpdateSnapshot(token, updatedScaleAmount, syncDate,
            _timeProviderMock.Object);

        position.AddOrUpdateSnapshot(token, updatedScaleAmount, syncDate.AddDays(1),
            _timeProviderMock.Object);

        Assert.Single(position.PositionEvents);
        AssertThatAaveEventCorrect(
            position,
            position.PositionEvents.First(),
            position.Id,
            token,
            initialScaleAmount,
            eventType);
    }

    [Theory]
    [InlineData(100, 100, AavePositionEventType.Deposit)]
    [InlineData(100, 100, AavePositionEventType.Withdrawal)]
    public void AddOrUpdateSnapshotTest_WhenScaleChange_ShouldUpdateSnapshot(
        decimal oldScaleAmount,
        decimal newScaleAmount,
        AavePositionEventType eventType)
    {
        var position = CreatePosition(AavePositionType.Borrowed);
        var token = _fixture.Create<TokenInfo>();

        position.AddOrUpdateSnapshot(token, oldScaleAmount, TestDate, _timeProviderMock.Object);
        position.AddOrUpdateSnapshot(token, newScaleAmount, TestDate, _timeProviderMock.Object);

        Assert.Single(position.PositionEvents);
        AssertThatAaveEventCorrect(
            position,
            position.PositionEvents.Last(),
            position.Id,
            token,
            oldScaleAmount,
            eventType);
    }

    private AavePosition CreatePosition(AavePositionType type)
    {
        return new AavePosition(
            AaveNetwork.CeloNetwork,
            TestWallet,
            type,
            _fixture.Create<string>(),
            TestDate);
    }

    private static void AssertThatAaveEventCorrect(
        AavePosition position,
        AavePositionEvent @event,
        Guid positionId,
        TokenInfo eventToken,
        decimal positionScale,
        AavePositionEventType type)
    {
        var expectedToken = type == AavePositionEventType.Withdrawal
            ? eventToken with { Amount = (decimal)(positionScale - position.PreviousScaledAmount)! }
            : eventToken with { Amount = (decimal)(position.PreviousScaledAmount - positionScale)! };

        Assert.Equal(positionId, @event.PositionId);
        Assert.Equal(TestTime, @event.Date);
        Assert.Equal(expectedToken, @event.Token);
        Assert.Equal(type, @event.EventType);
    }
}