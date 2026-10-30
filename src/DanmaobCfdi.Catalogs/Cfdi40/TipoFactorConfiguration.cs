using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class TipoFactorConfiguration : IEntityTypeConfiguration<TipoFactor>
{
    public void Configure(EntityTypeBuilder<TipoFactor> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_TipoFactor", 6, 50, false);
    }
}
