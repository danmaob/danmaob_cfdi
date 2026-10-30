using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class ImpuestoConfiguration : IEntityTypeConfiguration<Impuesto>
{
    public void Configure(EntityTypeBuilder<Impuesto> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_Impuesto", 3, 50, true);
        builder.Property(entity => entity.LocalOFederal).HasMaxLength(50);
    }
}
