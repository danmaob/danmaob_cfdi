using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class PeriodicidadConfiguration : IEntityTypeConfiguration<Periodicidad>
{
    public void Configure(EntityTypeBuilder<Periodicidad> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_Periodicidad", 2, 50, true);
    }
}
