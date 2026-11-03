using System;
using System.Collections.Generic;

namespace DanmaobCfdi.Core.Reading.Cancelacion;

public sealed class CancelacionData
{
    public string RfcEmisor { get; internal init; } = string.Empty;
    public DateTime Fecha { get; internal init; }
    public IReadOnlyList<FolioData> Folios { get; internal init; } = Array.Empty<FolioData>();
}
