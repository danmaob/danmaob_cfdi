using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class NumPedimentoAduanaConfiguration : IEntityTypeConfiguration<NumPedimentoAduana>
{
    public void Configure(EntityTypeBuilder<NumPedimentoAduana> builder)
    {
        builder.ToTable("c_NumPedimentoAduana");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Aduana).HasMaxLength(2);
        builder.Property(entity => entity.Patente).HasMaxLength(4);
        builder.Property(entity => entity.FechaInicioVigencia).HasColumnType("date");
        builder.Property(entity => entity.FechaFinVigencia).HasColumnType("date");
        builder.HasIndex("Aduana", "Patente", "Ejercicio").IsUnique();
    }
}
