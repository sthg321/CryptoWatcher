using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Models;
using CryptoWatcher.Modules.Fluid.Infrastructure.Integrations.Blockchain.Abstractions;
using CryptoWatcher.ValueObjects;
using Nethereum.Contracts;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Fluid.Infrastructure.Integrations.Blockchain.Events;

public class DepositLogDecoder : IFluidTransactionLogDecoder
{
    public bool CanDecode(TransactionReceipt receipt)
    {
        return receipt.DecodeAllEvents<DepositEvent>().Count == 1;
    }

    public FluidEvent DecodeEventFromLog(TransactionReceipt receipt)
    {
        var transfer = receipt.DecodeAllEvents<TransferEventDTO>().First();

        var deposit = receipt.DecodeAllEvents<DepositEvent>().Single();

        return new FluidEvent
        {
            Token = new Token
            {
                Address = EvmAddress.Create(transfer.Log.Address),
                Balance = deposit.Event.Assets,
            },
            Shares = deposit.Event.Shares,
            EventType = CashFlowEvent.Deposit
        };
    }
}