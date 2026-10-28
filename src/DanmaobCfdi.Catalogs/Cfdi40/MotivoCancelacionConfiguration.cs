using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class MotivoCancelacionConfiguration : IEntityTypeConfiguration<MotivoCancelacion>
{
    public void Configure(EntityTypeBuilder<MotivoCancelacion> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_MotivoCancelacion", 2, 100, true);
    }
}
