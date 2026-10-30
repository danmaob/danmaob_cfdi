namespace DanmaobCfdi.Core.Reading.Pagos20;

public sealed class TotalesData
{
    public decimal? TotalRetencionesIVA { get; internal init; }

    public decimal? TotalRetencionesISR { get; internal init; }

    public decimal? TotalRetencionesIEPS { get; internal init; }

    public decimal? TotalTrasladosBaseIVA16 { get; internal init; }

    public decimal? TotalTrasladosImpuestoIVA16 { get; internal init; }

    public decimal? TotalTrasladosBaseIVA8 { get; internal init; }

    public decimal? TotalTrasladosImpuestoIVA8 { get; internal init; }

    public decimal? TotalTrasladosBaseIVA0 { get; internal init; }

    public decimal? TotalTrasladosImpuestoIVA0 { get; internal init; }

    public decimal? TotalTrasladosBaseIVAExento { get; internal init; }

    public decimal MontoTotalPagos { get; internal init; }
}
