using DanmaobCfdi.Catalogs.Persistence;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class CodigoTransporteAereo : SatCatalogEntity
{
    public string Nacionalidad { get; set; } = string.Empty;
    public string DesignadorOACI { get; set; } = string.Empty;
}
