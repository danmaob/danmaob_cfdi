using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class ConfigMaritimaConfiguration : IEntityTypeConfiguration<ConfigMaritima>
{
    public void Configure(EntityTypeBuilder<ConfigMaritima> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_ConfigMaritima", 3, 50, true);
    }
}
