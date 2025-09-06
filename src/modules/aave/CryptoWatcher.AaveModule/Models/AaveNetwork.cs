namespace CryptoWatcher.AaveModule.Models;

/// <summary>
/// Represents a network in the Aave protocol. This class is used to define
/// specific networks supported within the AaveModule and provides methods
/// to access these networks.
/// </summary>
public class AaveNetwork
{
    private const string Celo = nameof(Celo);
    private const string Sonic = nameof(Sonic);
    private const string Arbitrum = nameof(Arbitrum);
    
    private AaveNetwork(string network)
    {
        Name = network;
    }

    /// <summary>
    /// Gets the value representing the network name.
    /// </summary>
    public string Name { get; private set; }
    
    public static AaveNetwork CeloNetwork() => new(Celo);
    
    public static AaveNetwork SonicNetwork() => new(Sonic);

    public static IEnumerable<AaveNetwork> All => [CeloNetwork()];
}