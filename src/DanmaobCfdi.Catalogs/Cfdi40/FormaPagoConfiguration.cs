using DanmaobCfdi.Catalogs.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class FormaPagoConfiguration : IEntityTypeConfiguration<FormaPago>
{
    public void Configure(EntityTypeBuilder<FormaPago> builder)
    {
        SatCatalogConfiguration.ConfigureCatalog(builder, "c_FormaPago", 2, 50, true);
        builder.Property(entity => entity.Bancarizado).HasMaxLength(50);
        builder.Property(entity => entity.NumeroOperacion).HasMaxLength(50);
        builder.Property(entity => entity.RfcEmisorCuentaOrdenante).HasMaxLength(50);
        builder.Property(entity => entity.CuentaOrdenante).HasMaxLength(50);
        builder.Property(entity => entity.PatronCuentaOrdenante).HasMaxLength(100);
        builder.Property(entity => entity.RfcEmisorCuentaBeneficiario).HasMaxLength(50);
        builder.Property(entity => entity.CuentaBeneficiario).HasMaxLength(50);
        builder.Property(entity => entity.PatronCuentaBeneficiaria).HasMaxLength(100);
        builder.Property(entity => entity.TipoCadenaPago).HasMaxLength(50);
        builder.Property(entity => entity.NombreBancoEmisorCuentaOrdenante).HasMaxLength(100);
    }
}
