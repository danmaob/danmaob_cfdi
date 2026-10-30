using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class CodigoPostalConfiguration : IEntityTypeConfiguration<CodigoPostal>
{
    public void Configure(EntityTypeBuilder<CodigoPostal> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_CodigoPostal", 5, 50, false);
        builder.Property(entity => entity.Estado).HasMaxLength(3);
        builder.Property(entity => entity.Municipio).HasMaxLength(3);
        builder.Property(entity => entity.Localidad).HasMaxLength(2);
        builder.Property(entity => entity.HusoHorario).HasMaxLength(50);
        builder.Property(entity => entity.MesInicioHorarioVerano).HasMaxLength(20);
        builder.Property(entity => entity.DiaInicioHorarioVerano).HasMaxLength(20);
        builder.Property(entity => entity.HoraInicioHorarioVerano).HasMaxLength(5);
        builder.Property(entity => entity.MesInicioHorarioInvierno).HasMaxLength(20);
        builder.Property(entity => entity.DiaInicioHorarioInvierno).HasMaxLength(20);
        builder.Property(entity => entity.HoraInicioHorarioInvierno).HasMaxLength(5);
    }
}
