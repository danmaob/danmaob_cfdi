using DanmaobCfdi.Catalogs.Persistence;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class Moneda : SatCatalogEntity
{
    public int Decimales { get; set; }
    public decimal? PorcentajeVariacion { get; set; }
}
