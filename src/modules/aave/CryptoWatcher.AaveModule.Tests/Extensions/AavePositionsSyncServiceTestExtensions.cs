using AutoFixture;
using CryptoWatcher.AaveModule.Entities;
using CryptoWatcher.AaveModule.Models;
using CryptoWatcher.AaveModule.Services;
using CryptoWatcher.AaveModule.Specifications;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Shared.ValueObjects;
using Moq;

namespace CryptoWatcher.AaveModule.Tests.Extensions;

internal static class AavePositionsSyncServiceTestExtensions
{
    public static void SetupEmptyListFromRepo(this Mock<IRepository<AavePosition>> mock)
    {
        mock.Setup(repository => repository.ListAsync(It.IsAny<AavePositionsWithSnapshotsSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
    }

    public static List<TokenInfo> SetupAaveTokenEnricher(this Mock<IAaveTokenEnricher> mock,
        Fixture fixture,
        AaveNetwork network,
        IEnumerable<AaveLendingPosition> expectedPositions)
    {
        var expectedSnapshotTokens = new List<TokenInfo>();
        foreach (var expectedPosition in expectedPositions)
        {
            var expectedTokenInfo = fixture.Create<TokenInfo>();
            mock.Setup(enricher => enricher.GetEnrichedTokenInfoAsync(network,
                    (CalculatableAaveLendingPosition)expectedPosition, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenInfo);
            expectedSnapshotTokens.Add(expectedTokenInfo);
        }

        return expectedSnapshotTokens;
    }
}