using System;

namespace DanmaobCfdi.Catalogs.Persistence;

internal abstract class SatCatalogEntity
{
    public string Clave { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateOnly? FechaInicioVigencia { get; set; }
    public DateOnly? FechaFinVigencia { get; set; }
}
