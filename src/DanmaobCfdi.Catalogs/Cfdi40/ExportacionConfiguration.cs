using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class ExportacionConfiguration : IEntityTypeConfiguration<Exportacion>
{
    public void Configure(EntityTypeBuilder<Exportacion> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_Exportacion", 2, 100, true);
    }
}
