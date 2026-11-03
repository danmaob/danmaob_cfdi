namespace DanmaobCfdi.Core.Reading.CartaPorte31;

public sealed class IdentificacionVehicularData
{
    public string ConfigVehicular { get; internal init; } = string.Empty;
    public decimal PesoBrutoVehicular { get; internal init; }
    public string PlacaVM { get; internal init; } = string.Empty;
    public int AnioModeloVM { get; internal init; }
}
