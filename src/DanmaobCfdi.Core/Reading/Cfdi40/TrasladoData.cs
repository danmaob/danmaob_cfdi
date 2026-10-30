namespace DanmaobCfdi.Core.Reading.Cfdi40;

public sealed class TrasladoData
{
    public decimal Base { get; internal init; }

    public string Impuesto { get; internal init; } = string.Empty;

    public string TipoFactor { get; internal init; } = string.Empty;

    public decimal? TasaOCuota { get; internal init; }

    public decimal? Importe { get; internal init; }
}
