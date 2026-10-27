using System;
using System.Collections.Generic;

namespace DanmaobCfdi.Core.Reading.Pagos20;

public sealed class PagoData
{
    public DateTime FechaPago { get; internal init; }
    public string FormaDePagoP { get; internal init; } = string.Empty;
    public string MonedaP { get; internal init; } = string.Empty;
    public decimal? TipoCambioP { get; internal init; }
    public decimal Monto { get; internal init; }
    public string? NumOperacion { get; internal init; }
    public IReadOnlyList<DoctoRelacionadoData> DoctosRelacionados { get; internal init; } = Array.Empty<DoctoRelacionadoData>();
}
