using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IUniswapEvent
{
    UniswapProtocolVersion ProtocolVersion { get; }
}