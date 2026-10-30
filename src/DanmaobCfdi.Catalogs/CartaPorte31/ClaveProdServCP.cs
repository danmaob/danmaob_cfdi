using DanmaobCfdi.Catalogs.Persistence;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class ClaveProdServCP : SatCatalogEntity
{
    public string? PalabrasSimilares { get; set; }
    public string MaterialPeligroso { get; set; } = string.Empty;
}
