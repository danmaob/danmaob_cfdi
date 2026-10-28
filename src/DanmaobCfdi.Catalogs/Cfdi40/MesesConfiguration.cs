using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class MesesConfiguration : IEntityTypeConfiguration<Meses>
{
    public void Configure(EntityTypeBuilder<Meses> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_Meses", 2, 50, true);
    }
}
