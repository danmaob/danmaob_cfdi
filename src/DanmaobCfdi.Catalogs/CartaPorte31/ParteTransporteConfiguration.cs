using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class ParteTransporteConfiguration : IEntityTypeConfiguration<ParteTransporte>
{
    public void Configure(EntityTypeBuilder<ParteTransporte> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_ParteTransporte", 4, 50, true);
    }
}
