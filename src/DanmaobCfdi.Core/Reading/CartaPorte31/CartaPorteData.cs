using System;
using System.Collections.Generic;

namespace DanmaobCfdi.Core.Reading.CartaPorte31;

public sealed class CartaPorteData
{
    public string Version { get; internal init; } = string.Empty;
    public string IdCCP { get; internal init; } = string.Empty;
    public string TranspInternac { get; internal init; } = string.Empty;
    public string? EntradaSalidaMerc { get; internal init; }
    public string? PaisOrigenDestino { get; internal init; }
    public string? ViaEntradaSalida { get; internal init; }
    public decimal? TotalDistRec { get; internal init; }
    public IReadOnlyList<UbicacionData> Ubicaciones { get; internal init; } = Array.Empty<UbicacionData>();
    public MercanciasData Mercancias { get; internal init; } = new MercanciasData();
    public IReadOnlyList<TiposFiguraData> FiguraTransporte { get; internal init; } = Array.Empty<TiposFiguraData>();
}
