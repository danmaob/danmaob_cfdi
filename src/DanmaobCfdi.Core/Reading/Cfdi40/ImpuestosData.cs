using System;
using System.Collections.Generic;

namespace DanmaobCfdi.Core.Reading.Cfdi40;

public sealed class ImpuestosData
{
    public decimal? TotalImpuestosRetenidos { get; internal init; }

    public decimal? TotalImpuestosTrasladados { get; internal init; }

    public IReadOnlyList<RetencionData> Retenciones { get; internal init; } = Array.Empty<RetencionData>();

    public IReadOnlyList<TrasladoData> Traslados { get; internal init; } = Array.Empty<TrasladoData>();
}
