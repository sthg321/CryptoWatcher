using Microsoft.Extensions.DependencyInjection;

namespace CoinGeckoClient.Extensions;

public static class ServiceCollectionExtensions
{
    private const string BaseUrl = "https://api.coingecko.com";

    public static void AddCoinGeckoClient(this IServiceCollection services,
        Func<IServiceProvider, Uri>? coinGeckoUriFactory = null)
    {
        services.AddHttpClient<ICoinGeckoApiClient, CoinGeckoApiClient>((provider, client) =>
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "crypto_tracker");
            client.BaseAddress = coinGeckoUriFactory?.Invoke(provider) ?? new Uri(BaseUrl);
        });
    }
}