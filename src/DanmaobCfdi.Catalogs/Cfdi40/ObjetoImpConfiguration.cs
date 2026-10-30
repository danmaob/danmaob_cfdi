using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class ObjetoImpConfiguration : IEntityTypeConfiguration<ObjetoImp>
{
    public void Configure(EntityTypeBuilder<ObjetoImp> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_ObjetoImp", 2, 100, true);
    }
}
