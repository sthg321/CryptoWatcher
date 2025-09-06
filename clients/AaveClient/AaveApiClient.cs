using AaveClient.Pool;
using AaveClient.UiPoolDataProvider;

namespace AaveClient;

public interface IAaveApiClient
{
    IUiPoolDataProviderFetcher UiPoolDataProviderFetcher { get; }

    IPoolFetcher PoolFetcher { get; }
}

public class AaveApiClient : IAaveApiClient
{
    public AaveApiClient(IUiPoolDataProviderFetcher uiPoolDataProviderFetcher, IPoolFetcher poolFetcher)
    {
        UiPoolDataProviderFetcher = uiPoolDataProviderFetcher;
        PoolFetcher = poolFetcher;
    }

    public IUiPoolDataProviderFetcher UiPoolDataProviderFetcher { get; }

    public IPoolFetcher PoolFetcher { get; }
}