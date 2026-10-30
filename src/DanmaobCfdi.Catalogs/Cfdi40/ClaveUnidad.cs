using DanmaobCfdi.Catalogs.Persistence;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class ClaveUnidad : SatCatalogEntity
{
    public string? DescripcionUnidad { get; set; }
    public string? Nota { get; set; }
    public string? Simbolo { get; set; }
}
