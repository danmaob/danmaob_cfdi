using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class ColoniaConfiguration : IEntityTypeConfiguration<Colonia>
{
    public void Configure(EntityTypeBuilder<Colonia> builder)
    {
        SatCatalogConfiguration.ConfigureColumns(builder, "c_Colonia", 4, 100, true);
        builder.HasKey("Clave", "CodigoPostal");
        builder.Property(entity => entity.CodigoPostal).HasMaxLength(5);
    }
}
