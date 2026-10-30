using DanmaobCfdi.Catalogs.Persistence;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class Estaciones : SatCatalogEntity
{
    public string ClaveTransporte { get; set; } = string.Empty;
    public string Nacionalidad { get; set; } = string.Empty;
    public string? DesignadorIATA { get; set; }
    public string? LineaFerrea { get; set; }
}
