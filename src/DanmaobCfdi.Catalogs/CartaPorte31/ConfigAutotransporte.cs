using DanmaobCfdi.Catalogs.Persistence;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class ConfigAutotransporte : SatCatalogEntity
{
    public int? NumeroEjes { get; set; }
    public string? NumeroLlantas { get; set; }
    public string Remolque { get; set; } = string.Empty;
}
