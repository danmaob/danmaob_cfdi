using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class RegistroISTMOConfiguration : IEntityTypeConfiguration<RegistroISTMO>
{
    public void Configure(EntityTypeBuilder<RegistroISTMO> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_RegistroISTMO", 2, 50, true);
    }
}
