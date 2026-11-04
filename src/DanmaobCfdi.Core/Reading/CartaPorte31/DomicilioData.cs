namespace DanmaobCfdi.Core.Reading.CartaPorte31;

public sealed class DomicilioData
{
    public string? Calle { get; internal init; }
    public string? NumeroExterior { get; internal init; }
    public string? NumeroInterior { get; internal init; }
    public string? Colonia { get; internal init; }
    public string? Localidad { get; internal init; }
    public string? Referencia { get; internal init; }
    public string? Municipio { get; internal init; }
    public string Estado { get; internal init; } = string.Empty;
    public string Pais { get; internal init; } = string.Empty;
    public string CodigoPostal { get; internal init; } = string.Empty;
}
