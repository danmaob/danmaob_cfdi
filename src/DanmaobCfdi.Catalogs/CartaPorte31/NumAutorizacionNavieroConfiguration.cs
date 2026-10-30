using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class NumAutorizacionNavieroConfiguration : IEntityTypeConfiguration<NumAutorizacionNaviero>
{
    public void Configure(EntityTypeBuilder<NumAutorizacionNaviero> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_NumAutorizacionNaviero", 15, 50, false);
    }
}
