using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class ContenedorConfiguration : IEntityTypeConfiguration<Contenedor>
{
    public void Configure(EntityTypeBuilder<Contenedor> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_Contenedor", 4, 50, true);
        builder.Property(entity => entity.TipoContenedor).HasMaxLength(50);
    }
}
