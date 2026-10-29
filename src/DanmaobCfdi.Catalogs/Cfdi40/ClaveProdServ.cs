using DanmaobCfdi.Catalogs.Persistence;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class ClaveProdServ : SatCatalogEntity
{
    public string IncluirIvaTrasladado { get; set; } = string.Empty;
    public string IncluirIepsTrasladado { get; set; } = string.Empty;
    public string? ComplementoQueDebeIncluir { get; set; }
    public byte EstimuloFranjaFronteriza { get; set; }
    public string? PalabrasSimilares { get; set; }
}
