using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class ContenedorMaritimoConfiguration : IEntityTypeConfiguration<ContenedorMaritimo>
{
    public void Configure(EntityTypeBuilder<ContenedorMaritimo> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_ContenedorMaritimo", 5, 50, true);
    }
}
