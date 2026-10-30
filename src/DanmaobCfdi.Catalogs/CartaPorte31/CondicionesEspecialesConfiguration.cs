using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class CondicionesEspecialesConfiguration : IEntityTypeConfiguration<CondicionesEspeciales>
{
    public void Configure(EntityTypeBuilder<CondicionesEspeciales> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_CondicionesEspeciales", 2, 50, true);
    }
}
