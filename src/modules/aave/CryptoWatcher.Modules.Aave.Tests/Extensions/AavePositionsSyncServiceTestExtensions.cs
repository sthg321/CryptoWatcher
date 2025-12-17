using AutoFixture;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Aave.Models;
using CryptoWatcher.Modules.Aave.Specifications;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;
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

    public static List<CryptoToken> SetupAaveTokenEnricher(this Mock<IAaveTokenEnricher> mock,
        Fixture fixture,
        AaveChainConfiguration chain,
        IEnumerable<AaveLendingPosition> expectedPositions)
    {
        var expectedSnapshotTokens = new List<CryptoToken>();
        foreach (var expectedPosition in expectedPositions)
        {
            var expectedTokenInfo = fixture.Create<CryptoToken>();
            
            mock.Setup(enricher => enricher.EnrichTokenAsync(chain,
                    (CalculatableAaveLendingPosition)expectedPosition, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTokenInfo);
            
            expectedSnapshotTokens.Add(expectedTokenInfo);
        }

        return expectedSnapshotTokens;
    }
}