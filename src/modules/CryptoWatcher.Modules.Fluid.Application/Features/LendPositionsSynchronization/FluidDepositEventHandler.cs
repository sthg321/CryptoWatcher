using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Exceptions;
using CryptoWatcher.Modules.Fluid.Abstractions;
using CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Models;
using CryptoWatcher.Modules.Fluid.Entities.Supply;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization;

public class FluidDepositEventHandler
{
    private readonly IFluidLendPositionRepository _positionRepository;
    private readonly ITokenEnricher _tokenEnricher;

    public FluidDepositEventHandler(IFluidLendPositionRepository positionRepository, ITokenEnricher tokenEnricher)
    {
        _positionRepository = positionRepository;
        _tokenEnricher = tokenEnricher;
    }

    public async Task HandleAsync(FluidEventDetails eventDetails)
    {
        var chainId = eventDetails.ChainId;
        var token = eventDetails.Event.Token;

        var activePosition =
            await _positionRepository.GetActivePositionAsync(chainId, token.Address, eventDetails.WalletAddress);

        var enrichedToken = await _tokenEnricher.EnrichAsync(chainId, token);

        activePosition = activePosition switch
        {
            null when eventDetails.Event.EventType == CashFlowEvent.Withdrawal => throw new DomainException(
                "Can't handle fluid withdraw event without position"),
            null => FluidLendPosition.Open(chainId,
                new CryptoTokenShort { Address = enrichedToken.Address, Symbol = enrichedToken.Symbol },
                eventDetails.WalletAddress),
            _ => activePosition
        };

        activePosition.AddCashFlow(eventDetails.Timestamp, eventDetails.Hash, enrichedToken.ToStatistic(),
            eventDetails.Event.EventType);
    }
}