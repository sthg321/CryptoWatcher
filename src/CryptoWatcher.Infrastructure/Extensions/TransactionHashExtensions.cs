using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Extensions;

public static class TransactionHashExtensions
{
    private const int TransactionHashLength = 66;

    public static void ConfigureTransactionHash(this PropertyBuilder<TransactionHash> builder)
    {
        builder
            .HasConversion(hash => hash.Value, hash => TransactionHash.FromString(hash))
            .IsFixedLength()
            .HasMaxLength(TransactionHashLength);
    }
}