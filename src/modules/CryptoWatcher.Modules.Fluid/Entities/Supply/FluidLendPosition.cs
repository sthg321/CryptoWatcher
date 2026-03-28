using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Fluid.Entities.Supply;

public class FluidLendPosition
{
    private readonly List<FluidLendPositionSnapshot> _snapshots = [];
    private readonly List<FluidLendPositionCashFlow> _cashFlows = [];

    private FluidLendPosition()
    {
    }

    private FluidLendPosition(int chainId, CryptoTokenShort token, EvmAddress walletAddress)
    {
        Id = Guid.CreateVersion7();
        ChainId = chainId;
        Token = token;
        WalletAddress = walletAddress;
    }

    public Guid Id { get; private set; }

    public int ChainId { get; private set; }

    public CryptoTokenShort Token { get; private set; } = null!;

    public EvmAddress WalletAddress { get; private set; } = null!;

    public IReadOnlyCollection<FluidLendPositionSnapshot> Snapshots { get; private set; } = [];

    public IReadOnlyCollection<FluidLendPositionCashFlow> CashFlows => _cashFlows;

    public static FluidLendPosition Open(int chainId, CryptoTokenShort token, EvmAddress walletAddress)
    {
        return new FluidLendPosition(chainId, token, walletAddress);
    }

    public void AddCashFlow(DateTimeOffset timestamp, TransactionHash hash, CryptoTokenStatistic tokenStatistic,
        CashFlowEvent @event)
    {
        var existedEvent = _cashFlows.FirstOrDefault(x => x.Hash == hash);
        if (existedEvent is not null)
        {
            return;
        }

        _cashFlows.Add(new FluidLendPositionCashFlow(timestamp, hash, @event, tokenStatistic));
    }
}