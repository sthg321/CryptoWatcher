using System.Numerics;
using CryptoWatcher.ValueObjects;
using Nethereum.Contracts;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Extensions;

public static class EventLogExtensions
{
    extension(List<EventLog<TransferEventDTO>> log)
    {
        public (Token token0, Token token1) MapEventToTokens()
        {
            if (log.Count == 0)
            {
                throw new InvalidOperationException("Log is empty");
            }

            var token0 = log.First().MapEventToToken();

            var token1 = log.First(eventLog => eventLog.Log.Address != token0.Address).MapEventToToken();

            return (token0, token1);
        }

        public (Token token0, Token? token1) MapEventToTokens(BigInteger token0Amount, BigInteger token1Amount)
        {
            if (log.Count == 0)
            {
                throw new InvalidOperationException("Log is empty");
            }

            var token0 = log.First().MapEventToToken(token0Amount);

            var token1 = log.FirstOrDefault(eventLog => eventLog.Log.Address != token0.Address)
                ?.MapEventToToken(token1Amount);

            return (token0, token1);
        }
    }

    extension(EventLog<TransferEventDTO> log)
    {
        public Token MapEventToToken()
        {
            return new Token
            {
                Address = EvmAddress.Create(log.Log.Address),
                Balance = log.Event.Value
            };
        }

        private Token MapEventToToken(BigInteger amount)
        {
            return new Token
            {
                Address = EvmAddress.Create(log.Log.Address),
                Balance = amount
            };
        }
    }
}