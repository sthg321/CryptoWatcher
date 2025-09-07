using AaveClient.AaveOracle;
using AaveClient.Pool;
using AaveClient.UiPoolDataProvider;

namespace AaveClient;

public interface IAaveApiClient
{
    IAaveOracleFetcher OracleFetcher { get; }

    IPoolFetcher PoolFetcher { get; }

    IUiPoolDataProviderFetcher UiPoolDataProviderFetcher { get; }
}

public class AaveApiClient : IAaveApiClient
{
    public AaveApiClient(IAaveOracleFetcher oracleFetcher, IPoolFetcher poolFetcher,
        IUiPoolDataProviderFetcher uiPoolDataProviderFetcher)
    {
        OracleFetcher = oracleFetcher;
        PoolFetcher = poolFetcher;
        UiPoolDataProviderFetcher = uiPoolDataProviderFetcher;
    }

    public IUiPoolDataProviderFetcher UiPoolDataProviderFetcher { get; }

    public IPoolFetcher PoolFetcher { get; }

    public IAaveOracleFetcher OracleFetcher { get; }
}