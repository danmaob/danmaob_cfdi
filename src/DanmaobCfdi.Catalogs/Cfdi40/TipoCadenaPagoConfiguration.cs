using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class TipoCadenaPagoConfiguration : IEntityTypeConfiguration<TipoCadenaPago>
{
    public void Configure(EntityTypeBuilder<TipoCadenaPago> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_TipoCadenaPago", 2, 50, true);
    }
}
