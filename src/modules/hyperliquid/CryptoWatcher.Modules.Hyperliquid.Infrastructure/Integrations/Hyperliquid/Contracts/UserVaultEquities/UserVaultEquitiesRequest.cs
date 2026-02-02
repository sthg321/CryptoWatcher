namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Integrations.Hyperliquid.Contracts.UserVaultEquities;

public class UserVaultEquitiesRequest
{
    public UserVaultEquitiesRequest(string user)
    {
        User = user;
        Type = "userVaultEquities";
    }
    
    public string Type { get; private set; }

    public string User { get; set; }
}