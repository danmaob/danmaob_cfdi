using DanmaobCfdi.Catalogs.Persistence;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class Impuesto : SatCatalogEntity
{
    public bool Retencion { get; set; }
    public bool Traslado { get; set; }
    public string LocalOFederal { get; set; } = string.Empty;
}
