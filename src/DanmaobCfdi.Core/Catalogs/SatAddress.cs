namespace DanmaobCfdi.Core.Catalogs;

public sealed class SatAddress
{
    public SatAddress(string pais, string estado, string codigoPostal, string? municipio, string? localidad, string? colonia)
    {
        Pais = pais;
        Estado = estado;
        CodigoPostal = codigoPostal;
        Municipio = municipio;
        Localidad = localidad;
        Colonia = colonia;
    }

    public string Pais { get; }

    public string Estado { get; }

    public string CodigoPostal { get; }

    public string? Municipio { get; }

    public string? Localidad { get; }

    public string? Colonia { get; }
}
