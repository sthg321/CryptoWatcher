using AutoFixture;
using CryptoWatcher.AaveModule.Tests.Customizations;
using CryptoWatcher.AaveModule.Tests.Extensions;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Aave.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Application.Services;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Aave.Models;
using CryptoWatcher.Modules.Aave.Specifications;
using CryptoWatcher.Modules.Aave.Tests.Customizations;
using CryptoWatcher.Modules.Aave.ValueObjects;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;
using JetBrains.Annotations;
using Moq;

namespace CryptoWatcher.AaveModule.Tests.Services;

[TestSubject(typeof(AavePositionsSyncService))]
public class AavePositionsSyncServiceTest
{
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

    private static readonly Wallet TestWallet = new()
        { Address = EvmAddress.Create("0xcaBBa9e7f4b3A885C5aa069f88469ac711Dd4aCC") };

    private static readonly DateOnly SyncDay = DateOnly.FromDateTime(DateTime.Now);
    private static readonly DateTimeOffset TestTime = DateTimeOffset.Now;

    private readonly Mock<IAaveProvider> _aaveProviderMock = new();
    private readonly Mock<IAaveTokenEnricher> _tokenEnricherMock = new();
    private readonly Mock<IRepository<AavePosition>> _aavePositionRepositoryMock = new();
    private readonly Mock<TimeProvider> _timeProviderMock = new();
    private readonly Fixture _fixture;

    public AavePositionsSyncServiceTest()
    {
        _fixture = new Fixture();
        _fixture.WithTokenDecimalsRange();
        _fixture.Customize(new PositiveBigIntegerCustomization());
        _fixture.Customize(new EvmAddressCustomization());

        _aavePositionRepositoryMock.Setup(repository => repository.UnitOfWork)
            .Returns(new Mock<IUnitOfWork>().Object);

        _timeProviderMock.Setup(provider => provider.LocalTimeZone).Returns(TimeZoneInfo.Utc);
        _timeProviderMock.Setup(provider => provider.GetUtcNow()).Returns(TestTime);
    }

    [Fact]
    public async Task SyncPositionsAsyncTest_WhenAllPositionsEmpty_ShouldReturnEmptyList()
    {
        var randomSyncDay = DateOnly.FromDateTime(_fixture.Create<DateTime>());

        _aaveProviderMock.Setup(provider =>
                provider.GetLendingPositionAsync(TestNetwork, TestWallet, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_fixture.CreateMany<EmptyAaveLendingPosition>().Cast<AaveLendingPosition>().ToList());

        _aavePositionRepositoryMock.SetupEmptyListFromRepo();

        var service = CreateService();

        var result = await service.SyncPositionsAsync(TestNetwork, TestWallet, randomSyncDay,
            TestContext.Current.CancellationToken);

        Assert.Empty(result);
    }

    [Theory]
    [InlineData(AavePositionType.Borrowed, false)]
    [InlineData(AavePositionType.Supplied, false)]
    [InlineData(AavePositionType.Borrowed, true)]
    [InlineData(AavePositionType.Supplied, true)]
    public async Task SyncPositionsAsyncTest_WhenOnlyBorrowedOrSuppliedPositions_ShouldReturnAllPositions(
        AavePositionType expectedPositionType, bool positionsExists)
    {
        var expectedPositions = expectedPositionType switch
        {
            AavePositionType.Borrowed => _fixture
                .CreateMany<BorrowedAaveLendingPosition>()
                .Cast<AaveLendingPosition>()
                .ToList(),

            AavePositionType.Supplied => _fixture
                .CreateMany<SuppliedAaveLendingPosition>()
                .Cast<AaveLendingPosition>()
                .ToList(),
            _ => throw new ArgumentOutOfRangeException(nameof(expectedPositionType), expectedPositionType, null)
        };

        _aaveProviderMock.Setup(provider =>
                provider.GetLendingPositionAsync(TestNetwork, TestWallet, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPositions);

        if (positionsExists)
        {
            var existedPositions = expectedPositions.Select(position =>
                new AavePosition(TestNetwork, TestWallet, expectedPositionType, position.TokenAddress, SyncDay));

            _aavePositionRepositoryMock.Setup(repository =>
                    repository.ListAsync(It.IsAny<AavePositionsWithSnapshotsSpecification>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedPositions.ToList);
        }
        else
        {
            _aavePositionRepositoryMock.SetupEmptyListFromRepo();
        }

        var expectedSnapshotTokens =
            _tokenEnricherMock.SetupAaveTokenEnricher(_fixture, TestNetwork, expectedPositions);

        var service = CreateService();

        var actual = await service.SyncPositionsAsync(TestNetwork, TestWallet, SyncDay,
            TestContext.Current.CancellationToken);

        Assert.Equal(expectedPositions.Count, actual.Count);

        var expectedMap = expectedPositions
            .Zip(expectedSnapshotTokens, (pos, token) => new { pos.TokenAddress, Token = token })
            .ToDictionary(x => x.TokenAddress, x => x.Token);

        Assert.Equal(expectedPositions.Count, actual.Count);

        foreach (var actualPosition in actual)
        {
            AssertThatAavePositionValid(actualPosition, expectedPositionType);

            var expectedSnapshot = expectedMap[actualPosition.TokenAddress];

            var actualSnapshot = actualPosition.PositionSnapshots.First();
            AssertThatSnapshotValid(expectedSnapshot, actualSnapshot, actualPosition.Id);
        }
    }

    [Fact]
    public async Task SyncPositionsAsyncTest_WhenExistAllTypePositions_ShouldReturnNotEmptyPositions()
    {
        AaveLendingPosition emptyPosition = _fixture.Create<EmptyAaveLendingPosition>();
        AaveLendingPosition suppliedPosition = _fixture.Create<SuppliedAaveLendingPosition>();
        AaveLendingPosition borrowedPosition = _fixture.Create<BorrowedAaveLendingPosition>();

        AaveLendingPosition[] expectedPositions = [suppliedPosition, borrowedPosition];

        _aaveProviderMock.Setup(provider =>
                provider.GetLendingPositionAsync(TestNetwork, TestWallet, It.IsAny<CancellationToken>()))
            .ReturnsAsync([..expectedPositions, emptyPosition]);

        _aavePositionRepositoryMock.SetupEmptyListFromRepo();

        var expectedSnapshotTokens =
            _tokenEnricherMock.SetupAaveTokenEnricher(_fixture, TestNetwork, expectedPositions);

        var service = CreateService();

        var actual = await service.SyncPositionsAsync(TestNetwork, TestWallet, SyncDay,
            TestContext.Current.CancellationToken);

        Assert.Equal(expectedPositions.Length, actual.Count);

        for (var index = 0; index < actual.Count; index++)
        {
            var actualPosition = actual[index];
            var expectedPosition = expectedPositions[index];

            var expectedPositionType = ((CalculatableAaveLendingPosition)expectedPosition).DeterminePositionType();

            AssertThatAavePositionValid(actualPosition, expectedPositionType);

            var expectedSnapshot = expectedSnapshotTokens[index];
            var actualSnapshot = actualPosition.PositionSnapshots.First();

            AssertThatSnapshotValid(expectedSnapshot, actualSnapshot, actualPosition.Id);
        }
    }

    [Fact]
    public async Task SyncPositionsAsyncTest_WhenPositionInDbExist_AndInAaveClosed_ShouldClosePosition()
    {
        var dbPosition = new AavePosition(TestNetwork, TestWallet, AavePositionType.Borrowed,
            _fixture.Create<EvmAddress>(),
            SyncDay.AddDays(-1));

        _aaveProviderMock.Setup(provider =>
                provider.GetLendingPositionAsync(TestNetwork, TestWallet, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new EmptyAaveLendingPosition { TokenAddress = dbPosition.TokenAddress }]);

        _aavePositionRepositoryMock.Setup(repository => repository.ListAsync(
                It.IsAny<AavePositionsWithSnapshotsSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([dbPosition]);

        var service = CreateService();

        var actual = await service.SyncPositionsAsync(TestNetwork, TestWallet, SyncDay,
            TestContext.Current.CancellationToken);

        Assert.Single(actual);

    }

    private AavePositionsSyncService CreateService()
    {
        return new AavePositionsSyncService(_aaveProviderMock.Object, _tokenEnricherMock.Object,
            _aavePositionRepositoryMock.Object, _timeProviderMock.Object);
    }

    [AssertionMethod]
    private static void AssertThatAavePositionValid(AavePosition actualPosition, AavePositionType expectedPositionType)
    {
        Assert.Equal(TestNetwork.Name, actualPosition.Network);
        Assert.Equal(actualPosition.PositionType, expectedPositionType);

        Assert.Single(actualPosition.PositionSnapshots);
    }

    [AssertionMethod]
    private static void AssertThatSnapshotValid(CryptoToken expectedSnapshot, AavePositionSnapshot actualSnapshot,
        Guid actualPositionId)
    {
        Assert.Equal(SyncDay, actualSnapshot.Day);
        Assert.Equal(actualPositionId, actualSnapshot.PositionId);
        Assert.Equivalent(expectedSnapshot.ToStatistic(), actualSnapshot.Token0);
    }
}