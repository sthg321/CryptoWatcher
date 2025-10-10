using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CryptoWatcher.Infrastructure.Configuration.Converters;

public class WalletAddressConverter : ValueConverter<EvmAddress, string>
{
    public WalletAddressConverter() : base(address => address.Value, s => EvmAddress.Create(s))
    {
    }
}