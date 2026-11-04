namespace DanmaobCfdi.Core.Reading.CartaPorte31;

public sealed class MercanciaData
{
    public string BienesTransp { get; internal init; } = string.Empty;
    public string Descripcion { get; internal init; } = string.Empty;
    public decimal Cantidad { get; internal init; }
    public string ClaveUnidad { get; internal init; } = string.Empty;
    public string? Unidad { get; internal init; }
    public string? MaterialPeligroso { get; internal init; }
    public string? CveMaterialPeligroso { get; internal init; }
    public string? Embalaje { get; internal init; }
    public decimal PesoEnKg { get; internal init; }
    public decimal? ValorMercancia { get; internal init; }
    public string? Moneda { get; internal init; }
    public string? FraccionArancelaria { get; internal init; }
}
