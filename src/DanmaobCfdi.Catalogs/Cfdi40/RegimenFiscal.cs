using DanmaobCfdi.Catalogs.Persistence;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class RegimenFiscal : SatCatalogEntity
{
    public bool AplicaPersonaFisica { get; set; }
    public bool AplicaPersonaMoral { get; set; }
}
