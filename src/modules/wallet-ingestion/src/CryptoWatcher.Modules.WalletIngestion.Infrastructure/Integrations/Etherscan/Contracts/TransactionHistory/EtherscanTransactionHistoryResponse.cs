using System.Text.Json;

namespace CryptoWatcher.Modules.WalletIngestion.Infrastructure.Integrations.Etherscan.Contracts.TransactionHistory;

public class EtherscanTransactionHistoryResponse
{
    public string Status { get; set; } = null!;

    public string Message { get; set; } = null!;

    public JsonElement Result { get; init; }

    public bool IsSuccess => Message != "NOTOK";
}