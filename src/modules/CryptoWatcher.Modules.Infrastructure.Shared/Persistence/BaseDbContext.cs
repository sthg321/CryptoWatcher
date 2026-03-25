using CryptoWatcher.Modules.Infrastructure.Shared.Persistence.Conventions;
using CryptoWatcher.Modules.Infrastructure.Shared.Persistence.Converters;
using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CryptoWatcher.Modules.Infrastructure.Shared.Persistence;

public class BaseDbContext : DbContext
{
    private const byte EvmAddressLength = 42;
    private const byte TransactionHashLength = 66;

    public BaseDbContext(DbContextOptions options) : base(options)
    {
        
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ComplexProperties<CryptoToken>();
        configurationBuilder.ComplexProperties<CryptoTokenShort>();
        configurationBuilder.ComplexProperties<CryptoTokenStatistic>();
        
        configurationBuilder.Properties<EvmAddress>()
            .HaveConversion<EvmAddressConverter>()
            .AreFixedLength()
            .AreUnicode(false)
            .HaveMaxLength(EvmAddressLength);

        configurationBuilder.Properties<EvmAddress?>()
            .HaveConversion<EvmAddressConverter>()
            .AreFixedLength()
            .AreUnicode(false)
            .HaveMaxLength(EvmAddressLength);
        
        configurationBuilder.Properties<TransactionHash>()
            .HaveConversion<TransactionHashConverter>()
            .AreFixedLength()
            .AreUnicode(false)
            .HaveMaxLength(TransactionHashLength);

        configurationBuilder.Properties<TransactionHash?>()
            .HaveConversion<TransactionHashConverter>()
            .AreFixedLength()
            .AreUnicode(false)
            .HaveMaxLength(TransactionHashLength);
        
        configurationBuilder.Conventions.Add(_ => new TokenInfoSymbolMaxLengthConvention());
    }
}