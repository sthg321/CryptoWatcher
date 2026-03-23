using Bogus;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Tests.DataSets;
using CryptoWatcher.Modules.Uniswap.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Tests.Fakers;

public sealed class UniswapChainConfigurationFaker : Faker<UniswapChainConfiguration>
{
    public UniswapChainConfigurationFaker()
    {
        RuleFor(chainConfiguration => chainConfiguration.Name, faker => faker.Crypto().Network().Name);
        RuleFor(chainConfiguration => chainConfiguration.ChainId, faker => faker.Random.Int(0));
        RuleFor(chainConfiguration => chainConfiguration.RpcUrl, faker => faker.Crypto().Network().Rpc);
        RuleFor(chainConfiguration => chainConfiguration.RpcAuthToken, () => null);
        RuleFor(chainConfiguration => chainConfiguration.BlockscoutUrl, faker => faker.Crypto().Network().Blockscout);
        RuleFor(chainConfiguration => chainConfiguration.SmartContractAddresses, faker => new UniswapAddresses
        {
            MultiCall = faker.Crypto().EvmAddress(),
            PoolFactory = faker.Crypto().EvmAddress(),
            PositionManager = faker.Crypto().EvmAddress(),
            StateView = faker.Crypto().EvmAddress()
        });
        RuleFor(chainConfiguration => chainConfiguration.ProtocolVersion,
            faker => faker.PickRandom<UniswapProtocolVersion>());
    }
}