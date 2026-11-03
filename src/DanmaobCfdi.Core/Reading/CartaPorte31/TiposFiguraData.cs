using System;
using System.Collections.Generic;

namespace DanmaobCfdi.Core.Reading.CartaPorte31;

public sealed class TiposFiguraData
{
    public string TipoFigura { get; internal init; } = string.Empty;
    public string? RFCFigura { get; internal init; }
    public string? NumLicencia { get; internal init; }
    public string NombreFigura { get; internal init; } = string.Empty;
    public string? NumRegIdTribFigura { get; internal init; }
    public string? ResidenciaFiscalFigura { get; internal init; }
    public IReadOnlyList<string> PartesTransporte { get; internal init; } = Array.Empty<string>();
    public DomicilioData? Domicilio { get; internal init; }
}
