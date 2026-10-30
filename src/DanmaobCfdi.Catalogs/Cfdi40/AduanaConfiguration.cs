using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class AduanaConfiguration : IEntityTypeConfiguration<Aduana>
{
    public void Configure(EntityTypeBuilder<Aduana> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_Aduana", 2, 100, true);
    }
}
