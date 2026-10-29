using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class MunicipioConfiguration : IEntityTypeConfiguration<Municipio>
{
    public void Configure(EntityTypeBuilder<Municipio> builder)
    {
        SatCatalogConfiguration.ConfigureColumns(builder, "c_Municipio", 3, 100, true);
        builder.HasKey("Clave", "Estado");
        builder.Property(entity => entity.Estado).HasMaxLength(3);
    }
}
