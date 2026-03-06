using System.Text.Json;

namespace CryptoWatcher.Modules.WalletIngestion.Infrastructure.Integrations.Etherscan.Contracts.TransactionHistory;

public class EtherscanTransactionHistoryResponse
{
    private const string SuccessStatus = "1";
    
    public string Status { get; set; } = null!;

    public string Message { get; set; } = null!;

    public JsonElement Result { get; init; }

    public bool IsSuccess => Status == SuccessStatus;
}