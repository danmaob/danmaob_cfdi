using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class MetodoPagoConfiguration : IEntityTypeConfiguration<MetodoPago>
{
    public void Configure(EntityTypeBuilder<MetodoPago> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_MetodoPago", 3, 50, true);
    }
}
