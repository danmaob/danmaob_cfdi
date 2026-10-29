using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Persistence;

internal static class SatCatalogConfiguration
{
    internal const string DescriptionCollation = "Latin1_General_100_CI_AI";

    internal static void ConfigureCatalog<TEntity>(EntityTypeBuilder<TEntity> builder, string tableName, int keyLength, int descriptionLength, bool descriptionRequired) where TEntity : SatCatalogEntity
    {
        ConfigureColumns(builder, tableName, keyLength, descriptionLength, descriptionRequired);
        builder.HasKey(entity => entity.Clave);
    }

    internal static void ConfigureColumns<TEntity>(EntityTypeBuilder<TEntity> builder, string tableName, int keyLength, int descriptionLength, bool descriptionRequired) where TEntity : SatCatalogEntity
    {
        builder.ToTable(tableName);
        builder.Property(entity => entity.Clave).HasMaxLength(keyLength);
        builder.Property(entity => entity.Descripcion).HasMaxLength(descriptionLength).IsRequired(descriptionRequired).UseCollation(DescriptionCollation);
        builder.Property(entity => entity.FechaInicioVigencia).HasColumnType("date");
        builder.Property(entity => entity.FechaFinVigencia).HasColumnType("date");
    }
}
