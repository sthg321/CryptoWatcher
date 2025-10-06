using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace CryptoWatcher.UniswapModule.Services;

public class UniswapLiquidityPoolTracker
{
    public async Task StartTrackAsync()
    {
        var web3 = new Web3("https://unichain-mainnet.infura.io/v3/8556559626d3455da401e9fd058cc591");

     
        var poolManagerAddress = "0x1f98400000000000000000000000000000000004";
        var myWallet = "0xeb9191d780c0aB6Ab320C5F05E41ebF81f14255f"; 
        
        
        var lastBlock = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
        
        var filter = new NewFilterInput
        {
            FromBlock = new BlockParameter(28813747),
            ToBlock = new BlockParameter(28813747),
            Address = new[] { poolManagerAddress },
            Topics = new object[]
            {
                // "0xf208f4912782fd25c7f114ca3723a2d5dd6f3bcc3ac8db5af63baa85f711d5ec" // ModifyLiquidity сигнатура
            }
        };
    }
}