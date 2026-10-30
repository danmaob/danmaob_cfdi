using DanmaobCfdi.Catalogs.Persistence;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class DerechosDePaso : SatCatalogEntity
{
    public string Entre { get; set; } = string.Empty;
    public string Hasta { get; set; } = string.Empty;
    public string OtorgaRecibe { get; set; } = string.Empty;
    public string Concesionario { get; set; } = string.Empty;
}
