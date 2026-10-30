namespace DanmaobCfdi.Core.Reading.Cfdi40;

public sealed class RetencionData
{
    public decimal? Base { get; internal init; }

    public string Impuesto { get; internal init; } = string.Empty;

    public string? TipoFactor { get; internal init; }

    public decimal? TasaOCuota { get; internal init; }

    public decimal Importe { get; internal init; }
}
