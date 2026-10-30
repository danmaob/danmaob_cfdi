using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class TipoEmbalajeConfiguration : IEntityTypeConfiguration<TipoEmbalaje>
{
    public void Configure(EntityTypeBuilder<TipoEmbalaje> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_TipoEmbalaje", 4, 250, true);
    }
}
