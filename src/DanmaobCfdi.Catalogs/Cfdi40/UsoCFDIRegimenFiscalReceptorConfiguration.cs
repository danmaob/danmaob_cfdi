using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class UsoCFDIRegimenFiscalReceptorConfiguration : IEntityTypeConfiguration<UsoCFDIRegimenFiscalReceptor>
{
    public void Configure(EntityTypeBuilder<UsoCFDIRegimenFiscalReceptor> builder)
    {
        builder.ToTable("c_UsoCFDI_RegimenFiscalReceptor");
        builder.HasKey("UsoCFDI", "RegimenFiscal");
        builder.Property(e => e.UsoCFDI).HasMaxLength(4);
        builder.Property(e => e.RegimenFiscal).HasMaxLength(3);
        builder.HasOne<UsoCFDI>().WithMany().HasForeignKey(e => e.UsoCFDI);
    }
}
