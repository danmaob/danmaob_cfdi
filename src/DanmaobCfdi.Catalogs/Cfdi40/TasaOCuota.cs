using System;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class TasaOCuota
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public decimal? ValorMinimo { get; set; }
    public decimal ValorMaximo { get; set; }
    public string Impuesto { get; set; } = string.Empty;
    public string Factor { get; set; } = string.Empty;
    public bool Traslado { get; set; }
    public bool Retencion { get; set; }
    public DateOnly? FechaInicioVigencia { get; set; }
    public DateOnly? FechaFinVigencia { get; set; }
}
