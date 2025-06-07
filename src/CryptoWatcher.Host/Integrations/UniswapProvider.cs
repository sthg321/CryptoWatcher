using CryptoWatcher.Entities;
using CryptoWatcher.Integrations;
using CryptoWatcher.Models;
using Nethereum.Web3;
using UniswapClient.Models;

namespace CryptoWatcher.Host.Integrations;

public class UniswapProvider : IUniswapProvider
{
    private readonly CryptoWatcher.Application.Uniswap.UniswapProvider _provider;

    public UniswapProvider(Application.Uniswap.UniswapProvider provider)
    {
        _provider = provider;
    }

    public async Task<LiquidityPool> GetPoolAsync(Network network, IUniswapPosition uniswapPosition)
    {
        return await _provider.GetPoolAsync(new Web3(network.RpcUrl), network, uniswapPosition);
    }

    public Task<PositionInPool> GetPoolPositionAsync(LiquidityPool pool)
    {
        throw new NotImplementedException();
    }

    public Task<TokenPair> GetPoolFeeAsync(LiquidityPool pool)
    {
        throw new NotImplementedException();
    }
}