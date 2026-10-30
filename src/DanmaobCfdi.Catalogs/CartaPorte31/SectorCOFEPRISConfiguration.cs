using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class SectorCOFEPRISConfiguration : IEntityTypeConfiguration<SectorCOFEPRIS>
{
    public void Configure(EntityTypeBuilder<SectorCOFEPRIS> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_SectorCOFEPRIS", 2, 50, true);
    }
}
