using System;
using System.Collections.Generic;

namespace DanmaobCfdi.Core.Reading.Cfdi40;

public sealed class ComprobanteData
{
    public string Version { get; internal init; } = string.Empty;

    public string? Serie { get; internal init; }

    public string? Folio { get; internal init; }

    public DateTime Fecha { get; internal init; }

    public string? FormaPago { get; internal init; }

    public string? CondicionesDePago { get; internal init; }

    public decimal SubTotal { get; internal init; }

    public decimal? Descuento { get; internal init; }

    public string Moneda { get; internal init; } = string.Empty;

    public decimal? TipoCambio { get; internal init; }

    public decimal Total { get; internal init; }

    public string TipoDeComprobante { get; internal init; } = string.Empty;

    public string Exportacion { get; internal init; } = string.Empty;

    public string? MetodoPago { get; internal init; }

    public string LugarExpedicion { get; internal init; } = string.Empty;

    public IReadOnlyList<CfdiRelacionadosData> CfdiRelacionados { get; internal init; } = Array.Empty<CfdiRelacionadosData>();

    public EmisorData Emisor { get; internal init; } = new EmisorData();

    public ReceptorData Receptor { get; internal init; } = new ReceptorData();

    public IReadOnlyList<ConceptoData> Conceptos { get; internal init; } = Array.Empty<ConceptoData>();

    public ImpuestosData? Impuestos { get; internal init; }
}
