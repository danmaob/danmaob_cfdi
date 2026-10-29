using System;

namespace DanmaobCfdi.Catalogs.Cfdi40;

internal sealed class NumPedimentoAduana
{
    public int Id { get; set; }
    public string Aduana { get; set; } = string.Empty;
    public string Patente { get; set; } = string.Empty;
    public int Ejercicio { get; set; }
    public int Cantidad { get; set; }
    public DateOnly? FechaInicioVigencia { get; set; }
    public DateOnly? FechaFinVigencia { get; set; }
}
