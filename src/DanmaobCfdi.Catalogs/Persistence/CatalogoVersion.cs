using System;

namespace DanmaobCfdi.Catalogs.Persistence;

internal sealed class CatalogoVersion
{
    public string Catalogo { get; set; } = string.Empty;
    public string? VersionSat { get; set; }
    public string? RevisionSat { get; set; }
    public DateOnly? FechaPublicacionSat { get; set; }
    public string VersionComponente { get; set; } = string.Empty;
    public DateTime FechaAplicacionUtc { get; set; }
}
