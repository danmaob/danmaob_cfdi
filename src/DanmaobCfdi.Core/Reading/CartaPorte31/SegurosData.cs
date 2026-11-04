namespace DanmaobCfdi.Core.Reading.CartaPorte31;

public sealed class SegurosData
{
    public string AseguraRespCivil { get; internal init; } = string.Empty;
    public string PolizaRespCivil { get; internal init; } = string.Empty;
    public string? AseguraMedAmbiente { get; internal init; }
    public string? PolizaMedAmbiente { get; internal init; }
    public string? AseguraCarga { get; internal init; }
    public string? PolizaCarga { get; internal init; }
    public decimal? PrimaSeguro { get; internal init; }
}
