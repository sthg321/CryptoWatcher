using AutoFixture;
using CryptoWatcher.AaveModule.Abstractions;
using CryptoWatcher.AaveModule.Entities;
using CryptoWatcher.AaveModule.Models;
using CryptoWatcher.AaveModule.Services;
using CryptoWatcher.AaveModule.Specifications;
using CryptoWatcher.AaveModule.Tests.Extensions;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.Shared.ValueObjects;
using JetBrains.Annotations;
using Moq;

namespace CryptoWatcher.AaveModule.Tests.Services;

[TestSubject(typeof(AavePositionsSyncService))]
public class AavePositionsSyncServiceTest
{
    private static readonly AaveNetwork TestNetwork = AaveNetwork.CeloNetwork;
    private static readonly Wallet TestWallet = new() { Address = Guid.CreateVersion7().ToString() };
    private static readonly DateOnly SyncDay = DateOnly.FromDateTime(DateTime.Now);

    private readonly Mock<IAaveProvider> _aaveProviderMock = new();
    private readonly Mock<IAaveTokenEnricher> _tokenEnricherMock = new();
    private readonly Mock<IRepository<AavePosition>> _aavePositionRepositoryMock = new();

    public AavePositionsSyncServiceTest()
    {
        _aavePositionRepositoryMock.Setup(repository => repository.UnitOfWork)
            .Returns(new Mock<IUnitOfWork>().Object);
    }

    [Fact]
    public async Task SyncPositionsAsyncTest_WhenAllPositionsEmpty_ShouldReturnEmptyList()
    {
        var fixture = new Fixture();

        var randomSyncDay = DateOnly.FromDateTime(fixture.Create<DateTime>());

        _aaveProviderMock.Setup(provider =>
                provider.GetLendingPositionAsync(TestNetwork, TestWallet, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.CreateMany<EmptyAaveLendingPosition>().Cast<AaveLendingPosition>().ToList());

        _aavePositionRepositoryMock.SetupEmptyListFromRepo();

        var service = CreateService();

        var result = await service.SyncPositionsAsync(AaveNetwork.CeloNetwork, TestWallet, randomSyncDay,
            TestContext.Current.CancellationToken);

        Assert.Empty(result);
    }

    [Theory]
    [InlineData(AavePositionType.Borrowed)]
    [InlineData(AavePositionType.Supplied)]
    public async Task SyncPositionsAsyncTest_WhenOnlyBorrowedOrSuppliedPositions_ShouldReturnAllPositions(
        AavePositionType expectedPositionType)
    {
        var fixture = new Fixture();

        var expectedPositions = expectedPositionType switch
        {
            AavePositionType.Borrowed => fixture.CreateMany<BorrowedAaveLendingPosition>().Cast<AaveLendingPosition>()
                .ToList(),
            AavePositionType.Supplied => fixture.CreateMany<SuppliedAaveLendingPosition>().Cast<AaveLendingPosition>()
                .ToList(),
            _ => throw new ArgumentOutOfRangeException(nameof(expectedPositionType), expectedPositionType, null)
        };

        _aaveProviderMock.Setup(provider =>
                provider.GetLendingPositionAsync(TestNetwork, TestWallet, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPositions);

        _aavePositionRepositoryMock.SetupEmptyListFromRepo();

        var expectedSnapshotTokens = _tokenEnricherMock.SetupAaveTokenEnricher(fixture, TestNetwork, expectedPositions);

        var service = CreateService();

        var actual = await service.SyncPositionsAsync(AaveNetwork.CeloNetwork, TestWallet, SyncDay,
            TestContext.Current.CancellationToken);

        Assert.Equal(expectedPositions.Count, actual.Count);

        for (var index = 0; index < actual.Count; index++)
        {
            var actualPosition = actual[index];
            AssertThatAavePositionValid(actualPosition, expectedPositionType);

            var expectedSnapshot = expectedSnapshotTokens[index];
            var actualSnapshot = actualPosition.PositionSnapshots.First();

            AssertThatSnapshotValid(expectedSnapshot, actualSnapshot, actualPosition.Id);
            ;
        }
    }

    [Fact]
    public async Task SyncPositionsAsyncTest_WhenExistAllTypePositions_ShouldReturnNotEmptyPositions()
    {
        var fixture = new Fixture();

        AaveLendingPosition emptyPosition = fixture.Create<EmptyAaveLendingPosition>();
        AaveLendingPosition suppliedPosition = fixture.Create<SuppliedAaveLendingPosition>();
        AaveLendingPosition borrowedPosition = fixture.Create<BorrowedAaveLendingPosition>();

        AaveLendingPosition[] expectedPositions = [suppliedPosition, borrowedPosition];

        _aaveProviderMock.Setup(provider =>
                provider.GetLendingPositionAsync(TestNetwork, TestWallet, It.IsAny<CancellationToken>()))
            .ReturnsAsync([..expectedPositions, emptyPosition]);

        _aavePositionRepositoryMock.SetupEmptyListFromRepo();

        var expectedSnapshotTokens = _tokenEnricherMock.SetupAaveTokenEnricher(fixture, TestNetwork, expectedPositions);

        var service = CreateService();

        var actual = await service.SyncPositionsAsync(AaveNetwork.CeloNetwork, TestWallet, SyncDay,
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
        var fixture = new Fixture();

        var dbPosition = new AavePosition(TestNetwork, TestWallet, AavePositionType.Borrowed, fixture.Create<string>(),
            SyncDay.AddDays(-1));

        _aaveProviderMock.Setup(provider =>
                provider.GetLendingPositionAsync(TestNetwork, TestWallet, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new EmptyAaveLendingPosition { TokenAddress = dbPosition.TokenAddress }]);

        _aavePositionRepositoryMock.Setup(repository => repository.ListAsync(
                It.IsAny<AavePositionsWithSnapshotsSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([dbPosition]);

        var service = CreateService();

        var actual = await service.SyncPositionsAsync(AaveNetwork.CeloNetwork, TestWallet, SyncDay,
            TestContext.Current.CancellationToken);

        Assert.Single(actual);

        Assert.Equal(SyncDay, actual[0].ClosedAtDay);
    }

    private AavePositionsSyncService CreateService()
    {
        return new AavePositionsSyncService(_aaveProviderMock.Object, _tokenEnricherMock.Object,
            _aavePositionRepositoryMock.Object);
    }

    private static void AssertThatAavePositionValid(AavePosition actualPosition, AavePositionType expectedPositionType)
    {
        Assert.Equal(SyncDay, actualPosition.CreatedAtDay);
        Assert.Equal(TestNetwork.Name, actualPosition.Network);
        Assert.Equal(expectedPositionType, actualPosition.PositionType);
        Assert.Null(actualPosition.ClosedAtDay);

        Assert.Single(actualPosition.PositionSnapshots);
    }

    private static void AssertThatSnapshotValid(TokenInfo expectedSnapshot, AavePositionSnapshot actualSnapshot,
        Guid actualPositionId)
    {
        Assert.Equal(SyncDay, actualSnapshot.Day);
        Assert.Equal(actualPositionId, actualSnapshot.PositionId);
        Assert.Equivalent(expectedSnapshot, actualSnapshot.Token);
    }
}