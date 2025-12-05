using AutoFixture;
using CryptoWatcher.AaveModule.Tests.Customizations;
using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Extensions;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Aave.Models;
using CryptoWatcher.Modules.Aave.ValueObjects;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;
using JetBrains.Annotations;
using Moq;

namespace CryptoWatcher.AaveModule.Tests.Entities;

[TestSubject(typeof(AavePosition))]
public class AavePositionTest
{
    private static readonly Wallet TestWallet = new()
        { Address = EvmAddress.Create("0xb67dDd7562A4FeDAD0AbA02E09E696AAD365EcB7") };
    private static readonly AaveChainConfiguration TestNetwork = new AaveChainConfiguration
    {
        Name = "Celo",
        RpcUrl = new Uri("https://alfajores-forno.celo-testnet.org"),
        SmartContractAddresses = new AaveAddresses
        {
            PoolAddressesProviderAddress = EvmAddress.Create("0xcaBBa9e7f4b3A885C5aa069f88469ac711Dd4aCC"),
            UiPoolDataProviderAddress = EvmAddress.Create("0xcaBBa9e7f4b3A885C5aa069f88469ac711Dd4aCC")
        }
    };

    private static readonly DateTimeOffset TestTime = DateTimeOffset.UtcNow;
    private static readonly DateOnly TestDate = TestTime.DateTime.ToDateOnly();
    private readonly Fixture _fixture;
    private readonly Mock<TimeProvider> _timeProviderMock = new();

    public AavePositionTest()
    {
        _fixture = new Fixture();
        _fixture.Customize(new PositiveBigIntegerCustomization());
        _fixture.Customize(new EvmAddressCustomization());

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
    [InlineData(100, 200, AavePositionType.Borrowed, nameof(CashFlowEvent.Deposit))]
    [InlineData(500, 400, AavePositionType.Supplied, nameof(CashFlowEvent.Withdrawal))]
    [InlineData(100, 50, AavePositionType.Borrowed, nameof(CashFlowEvent.Withdrawal))]
    [InlineData(500, 600, AavePositionType.Supplied, nameof(CashFlowEvent.Deposit))]
    public void AddOrUpdateSnapshotTest_WhenScaleNotChange_ShouldAddSingleDepositEvent(decimal initialScaleAmount,
        decimal updatedScaleAmount,
        AavePositionType positionType,
        string eventName)
    {
        var eventType = CashFlowEvent.FromName(eventName);
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
    [InlineData(100, 150, nameof(CashFlowEvent.Deposit))]
    [InlineData(100, 50, nameof(CashFlowEvent.Withdrawal))]
    public void AddOrUpdateSnapshotTest_WhenScaleChange_ShouldUpdateSnapshot(
        decimal oldScaleAmount,
        decimal newScaleAmount,
        string eventName)
    {
        var eventType = CashFlowEvent.FromName(eventName);
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
            TestNetwork,
            TestWallet,
            type,
            _fixture.Create<EvmAddress>(),
            TestDate);
    }

    private static void AssertThatAaveEventCorrect(
        AavePosition position,
        AavePositionEvent @event,
        Guid positionId,
        TokenInfo eventToken,
        decimal positionScale,
        CashFlowEvent type)
    {
        var expectedToken = type == CashFlowEvent.Withdrawal
            ? eventToken with { Amount = (decimal)(positionScale - position.PreviousScaledAmount)! }
            : eventToken with { Amount = (decimal)(position.PreviousScaledAmount - positionScale)! };

        Assert.Equal(positionId, @event.PositionId);
        Assert.Equal(TestTime, @event.Date);
        Assert.Equal(expectedToken, @event.Token);
        Assert.Equal(type, @event.Event);
    }
}