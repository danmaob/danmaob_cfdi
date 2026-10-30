using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class MonedaConfiguration : IEntityTypeConfiguration<Moneda>
{
    public void Configure(EntityTypeBuilder<Moneda> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_Moneda", 3, 100, true);
        builder.Property(entity => entity.PorcentajeVariacion).HasPrecision(5, 2);
    }
}
