using System;
using System.Collections.Generic;

namespace DanmaobCfdi.Core.Reading.Cfdi40;

public sealed class CfdiRelacionadosData
{
    public string TipoRelacion { get; internal init; } = string.Empty;

    public IReadOnlyList<string> Uuids { get; internal init; } = Array.Empty<string>();
}
