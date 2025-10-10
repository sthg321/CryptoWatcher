using CryptoWatcher.Infrastructure.Configuration.Converters;
using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Extensions;

internal static class WalletAddressExtensions
{
    private const int WalletAddressLength = 42;
    
    public static void ConfigureEvmAddress(this PropertyBuilder<EvmAddress> builder)
    {
        builder
            .HasConversion<WalletAddressConverter>()
            .IsFixedLength()
            .HasMaxLength(WalletAddressLength);
    }
}