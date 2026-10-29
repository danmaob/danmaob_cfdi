using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class UsoCFDIConfiguration : IEntityTypeConfiguration<UsoCFDI>
{
    public void Configure(EntityTypeBuilder<UsoCFDI> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_UsoCFDI", 4, 100, true);
    }
}
