using System;
using System.Collections.Generic;

namespace DanmaobCfdi.Core.Reading.CartaPorte31;

public sealed class MercanciasData
{
    public decimal PesoBrutoTotal { get; internal init; }
    public string UnidadPeso { get; internal init; } = string.Empty;
    public decimal? PesoNetoTotal { get; internal init; }
    public int NumTotalMercancias { get; internal init; }
    public IReadOnlyList<MercanciaData> Mercancias { get; internal init; } = Array.Empty<MercanciaData>();
    public AutotransporteData? Autotransporte { get; internal init; }
}
