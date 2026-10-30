using DanmaobCfdi.Catalogs.Persistence;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class ClaveUnidadPeso : SatCatalogEntity
{
    public string? DescripcionUnidad { get; set; }
    public string? Nota { get; set; }
    public string? Simbolo { get; set; }
    public string Bandera { get; set; } = string.Empty;
}
