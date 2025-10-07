using Nethereum.Contracts.QueryHandlers.MultiCall;
using Nethereum.Web3;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Extensions;

internal static class Web3Extensions
{
    public static async Task<List<byte[]>> MultiCallAsync(
        this IWeb3 web3,
        List<Call> calls,
        string multiCallAddress)
    {
        var aggregateCall = new AggregateFunction
        {
            Calls = calls
        };

        var handler = web3.Eth.GetContractQueryHandler<AggregateFunction>();
        var outputDto = await handler.QueryDeserializingToObjectAsync<AggregateOutputDTO>(
            aggregateCall, multiCallAddress);

        return outputDto.ReturnData;
    }

    public static async Task<List<TOutput>> MultiCallAsync<TOutput>(
        this IWeb3 web3,
        List<Call> calls,
        string multiCallAddress,
        Func<byte[]?, TOutput> outputDecoder)
    {
        var aggregateCall = new AggregateFunction
        {
            Calls = calls
        };

        var handler = web3.Eth.GetContractQueryHandler<AggregateFunction>();
        var outputDto = await handler.QueryDeserializingToObjectAsync<AggregateOutputDTO>(
            aggregateCall, multiCallAddress);

        var result = new List<TOutput>(outputDto.ReturnData.Count);
        result.AddRange(outputDto.ReturnData.Select(outputDecoder));

        return result;
    }
}