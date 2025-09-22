using Microsoft.Extensions.DependencyInjection;
using Nethereum.ABI.ABIDeserialisation;
using UniswapClient.UniswapV3;
using UniswapClient.UniswapV3.LiquidityPool;
using UniswapClient.UniswapV3.LiquidityPoolFactory;
using UniswapClient.UniswapV3.PositionsFetcher;
using UniswapClient.UniswapV4;
using UniswapClient.UniswapV4.LiquidityPool;
using UniswapClient.UniswapV4.PositionsFetcher;
using UniswapClient.UniswapV4.StateView;
using UniswapClient.UniswapV4.UniswapAppApiClient;

namespace UniswapClient.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddUniswapClient(this IServiceCollection services)
    {
        AbiDeserializationSettings.UseSystemTextJson = true;
        //v3
        services.AddSingleton<UniswapV3Client>();
        services.AddSingleton<IUniswapV3LiquidityPool, UniswapV3LiquidityPool>();
        services.AddSingleton<IUniswapV3PoolFactory, UniswapV3PoolFactory>();
        services.AddSingleton<IUniswapV3PositionFetcher, UniswapV3PositionFetcher>();

        //v4
        services.AddSingleton<UniswapV4Client>();
        services.AddSingleton<IUniswapV4StateView, UniswapV4StateView>();
        services.AddSingleton<IUniswapV4LiquidityPool, UniswapV4LiquidityPool>();
        services.AddSingleton<IUniswapV4PositionFetcher, UniswapV4PositionFetcher>();
        services.AddSingleton<UniswapAppApiClient>(_ => new UniswapAppApiClient(new HttpClient
        {
            BaseAddress = new Uri("https://interface.gateway.uniswap.org")
        }));
    }
}