using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class TipoDeComprobanteConfiguration : IEntityTypeConfiguration<TipoDeComprobante>
{
    public void Configure(EntityTypeBuilder<TipoDeComprobante> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_TipoDeComprobante", 1, 50, true);
        builder.Property(entity => entity.ValorMaximo).HasPrecision(24, 6);
    }
}
