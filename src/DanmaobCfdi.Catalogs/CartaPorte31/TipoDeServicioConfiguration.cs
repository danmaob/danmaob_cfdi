using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class TipoDeServicioConfiguration : IEntityTypeConfiguration<TipoDeServicio>
{
    public void Configure(EntityTypeBuilder<TipoDeServicio> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_TipoDeServicio", 4, 50, true);
    }
}
