namespace DanmaobCfdi.Core.Reading.Pagos20;

public sealed class DoctoRelacionadoData
{
    public string IdDocumento { get; internal init; } = string.Empty;

    public string? Serie { get; internal init; }

    public string? Folio { get; internal init; }

    public string MonedaDR { get; internal init; } = string.Empty;

    public decimal? EquivalenciaDR { get; internal init; }

    public int NumParcialidad { get; internal init; }

    public decimal ImpSaldoAnt { get; internal init; }

    public decimal ImpPagado { get; internal init; }

    public decimal ImpSaldoInsoluto { get; internal init; }

    public string ObjetoImpDR { get; internal init; } = string.Empty;
}
