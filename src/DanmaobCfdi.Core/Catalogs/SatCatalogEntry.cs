using System;

namespace DanmaobCfdi.Core.Catalogs;

public sealed class SatCatalogEntry
{
    public SatCatalogEntry(string clave, string? clavePadre, string? descripcion, DateOnly? fechaInicioVigencia, DateOnly? fechaFinVigencia)
    {
        Clave = clave;
        ClavePadre = clavePadre;
        Descripcion = descripcion;
        FechaInicioVigencia = fechaInicioVigencia;
        FechaFinVigencia = fechaFinVigencia;
    }

    public string Clave { get; }

    public string? ClavePadre { get; }

    public string? Descripcion { get; }

    public DateOnly? FechaInicioVigencia { get; }

    public DateOnly? FechaFinVigencia { get; }
}
