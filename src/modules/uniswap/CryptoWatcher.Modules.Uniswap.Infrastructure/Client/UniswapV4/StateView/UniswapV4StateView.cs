using System.Numerics;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4.StateView.Contracts;
using Nethereum.ABI;
using Nethereum.Web3;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4.StateView;

internal interface IUniswapV4StateView
{
    Task<GetSlot0OutputDTO> GetSlot0Async(IWeb3 web3, UniswapV4PoolKey poolId25);

    Task<GetTickFeeGrowthOutsideOutput> GetTickInfoAsync(IWeb3 web3, UniswapV4PoolKey poolId25, int tick);

    Task<GetPositionInfoOutputDTO> GetPositionInfoAsync(IWeb3 web3, UniswapV4PoolKey poolId25,
        int tickLower, int tickUpper, ulong tokenId);

    Task<GetFeeGrowthGlobalsOutput> GetFeeGrowGlobalAsync(IWeb3 web3, UniswapV4PoolKey poolId25);
}

internal class UniswapV4StateView : IUniswapV4StateView
{
    private const string StateViewAddress = "0x86e8631a016f9068c3f085faf484ee3f5fdee8f2";
    private const string UniswapV4PositionsNft = "0x4529a01c7a0410167c5740c487a8de60232617bf";

    public async Task<GetSlot0OutputDTO> GetSlot0Async(IWeb3 web3, UniswapV4PoolKey poolId25)
    {
        var contract = web3.Eth.GetContract(UniswapV4StateViewAbi.Abi, StateViewAddress);

        var poolKey = GeneratePoolId(poolId25);

        var slot0 = await contract.GetFunction("getSlot0")
            .CallDeserializingToObjectAsync<GetSlot0OutputDTO>(poolKey);

        return slot0;
    }

    public async Task<GetTickFeeGrowthOutsideOutput> GetTickInfoAsync(IWeb3 web3, UniswapV4PoolKey poolId25, int tick)
    {
        var contract = web3.Eth.GetContract(UniswapV4StateViewAbi.Abi, StateViewAddress);

        var poolKey = GeneratePoolId(poolId25);

        return await contract.GetFunction("getTickInfo")
            .CallDeserializingToObjectAsync<GetTickFeeGrowthOutsideOutput>(poolKey, tick);
    }

    public async Task<GetPositionInfoOutputDTO> GetPositionInfoAsync(IWeb3 web3, UniswapV4PoolKey poolId25,
        int tickLower, int tickUpper, ulong tokenId)
    {
        var contract = web3.Eth.GetContract(UniswapV4StateViewAbi.Abi, StateViewAddress);

        var poolId = GeneratePoolId(poolId25);

        return await contract.GetFunction("getPositionInfo")
            .CallDeserializingToObjectAsync<GetPositionInfoOutputDTO>(poolId, UniswapV4PositionsNft, tickLower,
                tickUpper, ConvertTokenIdToBytes32(tokenId));
    }

    public async Task<GetFeeGrowthGlobalsOutput> GetFeeGrowGlobalAsync(IWeb3 web3, UniswapV4PoolKey poolId25)
    {
        var contract = web3.Eth.GetContract(UniswapV4StateViewAbi.Abi, StateViewAddress);

        var poolKey = GeneratePoolId(poolId25);

        return await contract.GetFunction("getFeeGrowthGlobals")
            .CallDeserializingToObjectAsync<GetFeeGrowthGlobalsOutput>(poolKey);
    }

    private static byte[] GeneratePoolId(UniswapV4PoolKey poolKey)
    {
        var abiEncode = new ABIEncode();

        return abiEncode.GetSha3ABIEncoded(
            new ABIValue("address", poolKey.Currency0),
            new ABIValue("address", poolKey.Currency1),
            new ABIValue("uint24", poolKey.Fee),
            new ABIValue("int24", poolKey.TickSpacing),
            new ABIValue("address", poolKey.Hooks)
        );
    }

    private static byte[] ConvertTokenIdToBytes32(BigInteger tokenId)
    {
        var bytes = tokenId.ToByteArray(isUnsigned: true, isBigEndian: true);
        var result = new byte[32];

        Buffer.BlockCopy(
            src: bytes,
            srcOffset: 0,
            dst: result,
            dstOffset: 32 - bytes.Length,
            count: bytes.Length
        );

        return result;
    }
}