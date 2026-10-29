using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class LocalidadConfiguration : IEntityTypeConfiguration<Localidad>
{
    public void Configure(EntityTypeBuilder<Localidad> builder)
    {
        SatCatalogConfiguration.ConfigureColumns(builder, "c_Localidad", 2, 50, true);
        builder.HasKey("Clave", "Estado");
        builder.Property(entity => entity.Estado).HasMaxLength(3);
    }
}
