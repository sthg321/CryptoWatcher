using Riok.Mapperly.Abstractions;

namespace CryptoWatcher.Modules.WalletIngestion.Infrastructure.Integrations.Etherscan.Contracts.TransactionHistory;

public class EtherscanTransactionHistoryQueryParams
{
    public EtherscanTransactionHistoryQueryParams()
    {
        Action = "txlist";
        Module = "account";
        Sort = "desc";
    }

    public string Address { get; set; } = null!;
    
    public string ApiKey { get; set; } = null!;

    public int ChainId { get; set; }

    [MapperIgnore] public string Module { get; private set; }

    [MapperIgnore] public string Action { get; set; }

    public int Page { get; set; }

    public int Offset { get; set; }

    public long StartBlock { get; set; }

    [MapperIgnore] public string Sort { get; set; }
}