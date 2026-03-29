namespace CryptoWatcher.Models;

public class BlockchainRegistry
{
    private readonly Dictionary<int, BlockchainNetwork> _networks = new List<BlockchainNetwork>
        {
            new()
            {
                Id = 1,
                Name = "Ethereum"
            },
            new()
            {
                Id = 42161,
                Name = "Arbitrum"
            },
            new()
            {
                Id = 143,
                Name = "Monad"
            },
            new()
            {
                Id = 8453,
                Name = "Base"
            },
            new()
            {
                Id = 130,
                Name = "Unichain"
            }
        }
        .ToDictionary(network => network.Id);

    public BlockchainNetwork GetNetwork(int id) => _networks.GetValueOrDefault(id) ?? throw new KeyNotFoundException(
                                                       $"Blockchain network with id: {id} not found");
}