using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class ClaveTipoCargaConfiguration : IEntityTypeConfiguration<ClaveTipoCarga>
{
    public void Configure(EntityTypeBuilder<ClaveTipoCarga> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_ClaveTipoCarga", 3, 50, true);
    }
}
