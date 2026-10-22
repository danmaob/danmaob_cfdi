namespace DanmaobCfdi.Core.Reading.Cfdi40;

public sealed class ReceptorData
{
    public string Rfc { get; internal init; } = string.Empty;

    public string Nombre { get; internal init; } = string.Empty;

    public string DomicilioFiscalReceptor { get; internal init; } = string.Empty;

    public string? ResidenciaFiscal { get; internal init; }

    public string? NumRegIdTrib { get; internal init; }

    public string RegimenFiscalReceptor { get; internal init; } = string.Empty;

    public string UsoCFDI { get; internal init; } = string.Empty;
}
