using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class TipoRelacionConfiguration : IEntityTypeConfiguration<TipoRelacion>
{
    public void Configure(EntityTypeBuilder<TipoRelacion> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_TipoRelacion", 2, 100, true);
    }
}
