using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class CodigoTransporteAereoConfiguration : IEntityTypeConfiguration<CodigoTransporteAereo>
{
    public void Configure(EntityTypeBuilder<CodigoTransporteAereo> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_CodigoTransporteAereo", 5, 100, true);
        builder.Property(entity => entity.Nacionalidad).HasMaxLength(50);
        builder.Property(entity => entity.DesignadorOACI).HasMaxLength(50);
    }
}
