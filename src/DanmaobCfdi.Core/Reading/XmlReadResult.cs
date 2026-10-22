using System;
using System.Collections.Generic;
using DanmaobCfdi.Core.Reading.Cfdi40;

namespace DanmaobCfdi.Core.Reading;

public sealed class XmlReadResult
{
    public XmlReadStatus Status { get; internal init; }

    public DocumentKind? Kind { get; internal init; }

    public ComprobanteData? Cfdi { get; internal init; }

    public IReadOnlyList<UnreadComplement> UnreadComplements { get; internal init; } = Array.Empty<UnreadComplement>();

    public UnsupportedReason? UnsupportedReason { get; internal init; }

    public string? DetectedVersion { get; internal init; }

    public IReadOnlyList<StructuralError> StructuralErrors { get; internal init; } = Array.Empty<StructuralError>();

    public RejectionReason? RejectionReason { get; internal init; }
}
