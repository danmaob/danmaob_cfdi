using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class DocumentoAduaneroConfiguration : IEntityTypeConfiguration<DocumentoAduanero>
{
    public void Configure(EntityTypeBuilder<DocumentoAduanero> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_DocumentoAduanero", 2, 250, true);
    }
}
