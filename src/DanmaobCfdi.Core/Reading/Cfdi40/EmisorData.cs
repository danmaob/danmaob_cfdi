namespace DanmaobCfdi.Core.Reading.Cfdi40;

public sealed class EmisorData
{
    public string Rfc { get; internal init; } = string.Empty;

    public string Nombre { get; internal init; } = string.Empty;

    public string RegimenFiscal { get; internal init; } = string.Empty;

    public string? FacAtrAdquirente { get; internal init; }
}
