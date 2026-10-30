using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class CveTransporteConfiguration : IEntityTypeConfiguration<CveTransporte>
{
    public void Configure(EntityTypeBuilder<CveTransporte> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_CveTransporte", 2, 50, true);
    }
}
