using System;

namespace DanmaobCfdi.Core.Reading.Cancelacion;

public sealed class FolioData
{
    public string UUID { get; internal init; } = string.Empty;
    public string UUIDEstatus { get; internal init; } = string.Empty;
    public string? UUIDdescripcion { get; internal init; }
    public DateTime? UUIDfecha { get; internal init; }
}
