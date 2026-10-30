using DanmaobCfdi.Catalogs.Persistence;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class Pais : SatCatalogEntity
{
    public string? FormatoCodigoPostal { get; set; }
    public string? FormatoRegistroIdentidadTributaria { get; set; }
    public string? ValidacionRegistroIdentidadTributaria { get; set; }
    public string? Agrupaciones { get; set; }
}
