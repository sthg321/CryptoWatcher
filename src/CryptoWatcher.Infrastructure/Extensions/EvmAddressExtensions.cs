using CryptoWatcher.Infrastructure.Configuration.Converters;
using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Extensions;

internal static class EvmAddressExtensions
{
    private const int WalletAddressLength = 42;
    
    public static void ConfigureEvmAddress(this PropertyBuilder<EvmAddress> builder)
    {
        builder
            .HasConversion<WalletAddressConverter>()
            .IsFixedLength()
            .HasMaxLength(WalletAddressLength);
    }
    
    public static void ConfigureEvmAddress(this ComplexTypePropertyBuilder<EvmAddress> builder)
    {
        builder
            .HasConversion<WalletAddressConverter>()
            .IsFixedLength()
            .HasMaxLength(WalletAddressLength);
    }
    
    public static void ConfigureNullableEvmAddress(this ComplexTypePropertyBuilder<EvmAddress> builder)
    {
        builder
            .HasConversion<WalletAddressConverter>()
            .IsFixedLength()
            .HasMaxLength(WalletAddressLength)
            .IsRequired(false);
    }
}