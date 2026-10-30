using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class TipoDeTraficoConfiguration : IEntityTypeConfiguration<TipoDeTrafico>
{
    public void Configure(EntityTypeBuilder<TipoDeTrafico> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_TipoDeTrafico", 4, 50, true);
    }
}
