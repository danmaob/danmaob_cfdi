using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class TasaOCuotaConfiguration : IEntityTypeConfiguration<TasaOCuota>
{
    public void Configure(EntityTypeBuilder<TasaOCuota> builder)
    {
        builder.ToTable("c_TasaOCuota");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Tipo).HasMaxLength(10);
        builder.Property(entity => entity.ValorMinimo).HasPrecision(18, 6);
        builder.Property(entity => entity.ValorMaximo).HasPrecision(18, 6);
        builder.Property(entity => entity.Impuesto).HasMaxLength(50);
        builder.Property(entity => entity.Factor).HasMaxLength(10);
        builder.Property(entity => entity.FechaInicioVigencia).HasColumnType("date");
        builder.Property(entity => entity.FechaFinVigencia).HasColumnType("date");
        builder.HasIndex("Tipo", "ValorMinimo", "ValorMaximo", "Impuesto", "Factor").IsUnique().HasFilter(null);
    }
}
