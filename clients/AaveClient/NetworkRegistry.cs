namespace AaveClient;

/// <summary>
/// https://aave.com/docs/resources/addresses
/// </summary>
public static class NetworkRegistry
{
    public record NetworkInfo(
        string UiPoolDataProviderAddress,
        string PoolAddressesProviderAddress,
        string PoolAddress);

    public static readonly Dictionary<AaveNetworkType, NetworkInfo> NetworkToRpcUrl = new()
    {
        [AaveNetworkType.Sonic] = new NetworkInfo("0x9005A69fE088680827f292e8aE885Be4BE1beb2f",
            "0x5C2e738F6E27bCE0F7558051Bf90605dD6176900", "0x3E59A31363E2ad014dcbc521c4a0d5757d9f3402"),

        [AaveNetworkType.Celo] = new NetworkInfo("0xf07fFd12b119b921C4a2ce8d4A13C5d1E3000d6e",
            "0x9F7Cf9417D5251C59fE94fB9147feEe1aAd9Cea5", "0x3E59A31363E2ad014dcbc521c4a0d5757d9f3402"),
    };
}