using DanmaobCfdi.Catalogs.Persistence;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class CodigoPostal : SatCatalogEntity
{
    public string Estado { get; set; } = string.Empty;
    public string? Municipio { get; set; }
    public string? Localidad { get; set; }
    public byte EstimuloFranjaFronteriza { get; set; }
    public string HusoHorario { get; set; } = string.Empty;
    public string? MesInicioHorarioVerano { get; set; }
    public string? DiaInicioHorarioVerano { get; set; }
    public string? HoraInicioHorarioVerano { get; set; }
    public int DiferenciaHorariaVerano { get; set; }
    public string? MesInicioHorarioInvierno { get; set; }
    public string? DiaInicioHorarioInvierno { get; set; }
    public string? HoraInicioHorarioInvierno { get; set; }
    public int DiferenciaHorariaInvierno { get; set; }
}
