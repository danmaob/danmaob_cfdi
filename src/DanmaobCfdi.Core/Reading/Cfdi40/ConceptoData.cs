using System;
using System.Collections.Generic;

namespace DanmaobCfdi.Core.Reading.Cfdi40;

public sealed class ConceptoData
{
    public string ClaveProdServ { get; internal init; } = string.Empty;

    public string? NoIdentificacion { get; internal init; }

    public decimal Cantidad { get; internal init; }

    public string ClaveUnidad { get; internal init; } = string.Empty;

    public string? Unidad { get; internal init; }

    public string Descripcion { get; internal init; } = string.Empty;

    public decimal ValorUnitario { get; internal init; }

    public decimal Importe { get; internal init; }

    public decimal? Descuento { get; internal init; }

    public string ObjetoImp { get; internal init; } = string.Empty;

    public IReadOnlyList<TrasladoData> Traslados { get; internal init; } = Array.Empty<TrasladoData>();

    public IReadOnlyList<RetencionData> Retenciones { get; internal init; } = Array.Empty<RetencionData>();
}
