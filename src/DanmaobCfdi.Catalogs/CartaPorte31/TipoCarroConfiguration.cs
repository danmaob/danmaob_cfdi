using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class TipoCarroConfiguration : IEntityTypeConfiguration<TipoCarro>
{
    public void Configure(EntityTypeBuilder<TipoCarro> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_TipoCarro", 4, 50, true);
    }
}
