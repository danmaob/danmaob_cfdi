using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Persistence;

internal sealed class CatalogoVersionConfiguration : IEntityTypeConfiguration<CatalogoVersion>
{
    public void Configure(EntityTypeBuilder<CatalogoVersion> builder)
    {
        builder.ToTable("CatalogoVersion");
        builder.HasKey(version => version.Catalogo);
        builder.Property(version => version.Catalogo).HasMaxLength(50);
        builder.Property(version => version.VersionSat).HasMaxLength(20);
        builder.Property(version => version.RevisionSat).HasMaxLength(20);
        builder.Property(version => version.FechaPublicacionSat).HasColumnType("date");
        builder.Property(version => version.VersionComponente).HasMaxLength(50);
        builder.Property(version => version.FechaAplicacionUtc).HasColumnType("datetime2");
    }
}
