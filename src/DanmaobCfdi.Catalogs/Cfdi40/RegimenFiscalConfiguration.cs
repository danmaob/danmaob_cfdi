using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class RegimenFiscalConfiguration : IEntityTypeConfiguration<RegimenFiscal>
{
    public void Configure(EntityTypeBuilder<RegimenFiscal> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_RegimenFiscal", 3, 100, true);
    }
}
