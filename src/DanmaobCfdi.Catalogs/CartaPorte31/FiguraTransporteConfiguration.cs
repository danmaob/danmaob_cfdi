using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class FiguraTransporteConfiguration : IEntityTypeConfiguration<FiguraTransporte>
{
    public void Configure(EntityTypeBuilder<FiguraTransporte> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_FiguraTransporte", 2, 50, true);
    }
}
