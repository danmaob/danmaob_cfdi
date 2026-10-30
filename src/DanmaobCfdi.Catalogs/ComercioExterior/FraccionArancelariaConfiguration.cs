using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.ComercioExterior;

internal sealed class FraccionArancelariaConfiguration : IEntityTypeConfiguration<FraccionArancelaria>
{
    public void Configure(EntityTypeBuilder<FraccionArancelaria> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_FraccionArancelaria", 10, 2000, true);
        builder.Property(entity => entity.UMT).HasMaxLength(2);
    }
}
