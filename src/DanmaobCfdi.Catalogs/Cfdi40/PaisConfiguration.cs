using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class PaisConfiguration : IEntityTypeConfiguration<Pais>
{
    public void Configure(EntityTypeBuilder<Pais> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_Pais", 3, 100, true);
        builder.Property(entity => entity.FormatoCodigoPostal).HasMaxLength(50);
        builder.Property(entity => entity.FormatoRegistroIdentidadTributaria).HasMaxLength(100);
        builder.Property(entity => entity.ValidacionRegistroIdentidadTributaria).HasMaxLength(50);
        builder.Property(entity => entity.Agrupaciones).HasMaxLength(50);
    }
}
