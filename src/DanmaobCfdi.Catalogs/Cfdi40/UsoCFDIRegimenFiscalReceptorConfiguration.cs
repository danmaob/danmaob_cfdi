using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class UsoCFDIRegimenFiscalReceptorConfiguration : IEntityTypeConfiguration<UsoCFDIRegimenFiscalReceptor>
{
    public void Configure(EntityTypeBuilder<UsoCFDIRegimenFiscalReceptor> builder)
    {
        builder.ToTable("c_UsoCFDI_RegimenFiscalReceptor");
        builder.HasKey("UsoCFDI", "RegimenFiscal");
        builder.Property(entity => entity.UsoCFDI).HasMaxLength(4);
        builder.Property(entity => entity.RegimenFiscal).HasMaxLength(3);
        builder.HasOne<UsoCFDI>().WithMany().HasForeignKey(entity => entity.UsoCFDI);
    }
}
