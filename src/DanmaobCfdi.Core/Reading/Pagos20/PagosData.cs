using System;
using System.Collections.Generic;

namespace DanmaobCfdi.Core.Reading.Pagos20;

public sealed class PagosData
{
    public string Version { get; internal init; } = string.Empty;
    public TotalesData Totales { get; internal init; } = new TotalesData();
    public IReadOnlyList<PagoData> Pagos { get; internal init; } = Array.Empty<PagoData>();
}
