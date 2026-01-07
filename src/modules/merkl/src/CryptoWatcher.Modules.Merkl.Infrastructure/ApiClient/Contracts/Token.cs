namespace CryptoWatcher.Modules.Merkl.Infrastructure.ApiClient.Contracts;

internal class Token
{
    public string Address { get; set; } = null!;

    public int ChainId { get; set; }

    public string Symbol { get; set; } = null!;

    public byte Decimals { get; set; }

    public decimal Price { get; set; }
}