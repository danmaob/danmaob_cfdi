using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class SubTipoRemConfiguration : IEntityTypeConfiguration<SubTipoRem>
{
    public void Configure(EntityTypeBuilder<SubTipoRem> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_SubTipoRem", 6, 50, true);
    }
}
