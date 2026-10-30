using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class DerechosDePasoConfiguration : IEntityTypeConfiguration<DerechosDePaso>
{
    public void Configure(EntityTypeBuilder<DerechosDePaso> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_DerechosDePaso", 6, 50, true);
        builder.Property(entity => entity.Entre).HasMaxLength(250);
        builder.Property(entity => entity.Hasta).HasMaxLength(250);
        builder.Property(entity => entity.OtorgaRecibe).HasMaxLength(50);
        builder.Property(entity => entity.Concesionario).HasMaxLength(250);
    }
}
