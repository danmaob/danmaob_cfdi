using DanmaobCfdi.Catalogs.Persistence;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class FormaPago : SatCatalogEntity
{
    public string Bancarizado { get; set; } = string.Empty;
    public string NumeroOperacion { get; set; } = string.Empty;
    public string RfcEmisorCuentaOrdenante { get; set; } = string.Empty;
    public string CuentaOrdenante { get; set; } = string.Empty;
    public string PatronCuentaOrdenante { get; set; } = string.Empty;
    public string RfcEmisorCuentaBeneficiario { get; set; } = string.Empty;
    public string CuentaBeneficiario { get; set; } = string.Empty;
    public string PatronCuentaBeneficiaria { get; set; } = string.Empty;
    public string TipoCadenaPago { get; set; } = string.Empty;
    public string NombreBancoEmisorCuentaOrdenante { get; set; } = string.Empty;
}
