using DanmaobCfdi.Catalogs.Persistence;

namespace DanmaobCfdi.Catalogs.CartaPorte31;

internal sealed class MaterialPeligroso : SatCatalogEntity
{
    public int Id { get; set; }
    public string? ClaseODivision { get; set; }
    public string? PeligroSecundario { get; set; }
    public string? NombreTecnico { get; set; }
}
