using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class FormaFarmaceuticaConfiguration : IEntityTypeConfiguration<FormaFarmaceutica>
{
    public void Configure(EntityTypeBuilder<FormaFarmaceutica> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_FormaFarmaceutica", 2, 50, true);
    }
}
