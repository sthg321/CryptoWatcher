using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace CryptoWatcher.Infrastructure.Configuration.Conventions;

/// <summary>
/// Sets max length for TokenInfo.Symbol to 16 characters across the model.
/// </summary>
public sealed class TokenInfoSymbolMaxLengthConvention : IModelFinalizingConvention
{
    internal const int MaxLength = 16;

    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context)
    {
        var model = modelBuilder.Metadata;

        foreach (var entityType in model.GetEntityTypes())
        {
            foreach (var complexProperty in entityType.GetComplexProperties())
            {
                var complexType = complexProperty.ComplexType;
                if (complexType.ClrType != typeof(CryptoToken))
                {
                    continue;
                }

                var symbolProperty = complexType.FindProperty(nameof(CryptoToken.Symbol));
                if (symbolProperty is null)
                {
                    continue;
                }

                symbolProperty.Builder.HasMaxLength(MaxLength);
                symbolProperty.Builder.IsUnicode(false);
            }
        }
    }
}