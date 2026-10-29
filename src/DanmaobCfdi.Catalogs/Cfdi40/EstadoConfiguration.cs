using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class EstadoConfiguration : IEntityTypeConfiguration<Estado>
{
    public void Configure(EntityTypeBuilder<Estado> builder)
    {
        SatCatalogConfiguration.ConfigureColumns(builder, "c_Estado", 3, 50, true);
        builder.HasKey("Clave", "Pais");
        builder.Property(entity => entity.Pais).HasMaxLength(3);
    }
}
