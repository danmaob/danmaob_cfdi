using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class MaterialPeligrosoConfiguration : IEntityTypeConfiguration<MaterialPeligroso>
{
    public void Configure(EntityTypeBuilder<MaterialPeligroso> builder)
    {
        SatCatalogConfiguration.ConfigureColumns(builder, "c_MaterialPeligroso", 4, 500, true);
        builder.HasKey(entity => entity.Id);
        builder.HasIndex(entity => entity.Clave);
        builder.Property(entity => entity.ClaseODivision).HasMaxLength(50);
        builder.Property(entity => entity.PeligroSecundario).HasMaxLength(50);
        builder.Property(entity => entity.NombreTecnico).HasMaxLength(250);
    }
}
