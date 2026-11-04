namespace DanmaobCfdi.Core.Catalogs;

public sealed class SatCodigoPostalEntry
{
    public SatCodigoPostalEntry(string clave, string estado, string? municipio, string? localidad)
    {
        Clave = clave;
        Estado = estado;
        Municipio = municipio;
        Localidad = localidad;
    }

    public string Clave { get; }

    public string Estado { get; }

    public string? Municipio { get; }

    public string? Localidad { get; }
}
