using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class RegimenAduaneroConfiguration : IEntityTypeConfiguration<RegimenAduanero>
{
    public void Configure(EntityTypeBuilder<RegimenAduanero> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_RegimenAduanero", 3, 250, true);
        builder.Property(entity => entity.ImpoExpo).HasMaxLength(50);
    }
}
