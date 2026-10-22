using System;
using System.Collections.Generic;

namespace DanmaobCfdi.Core.Reading;

internal sealed class DetectionResult
{
    private DetectionResult(DetectionOutcome outcome, DocumentKind? kind, UnsupportedReason? reason, string? detectedVersion, IReadOnlyList<UnreadComplement> unreadComplements)
    {
        Outcome = outcome;
        Kind = kind;
        Reason = reason;
        DetectedVersion = detectedVersion;
        UnreadComplements = unreadComplements;
    }

    public DetectionOutcome Outcome { get; }

    public DocumentKind? Kind { get; }

    public UnsupportedReason? Reason { get; }

    public string? DetectedVersion { get; }

    public IReadOnlyList<UnreadComplement> UnreadComplements { get; }

    public static DetectionResult Supported(DocumentKind kind, IReadOnlyList<UnreadComplement> unreadComplements)
    {
        return new DetectionResult(DetectionOutcome.Supported, kind, null, null, unreadComplements);
    }

    public static DetectionResult Unsupported(UnsupportedReason reason, string detectedVersion)
    {
        return new DetectionResult(DetectionOutcome.Unsupported, null, reason, detectedVersion, Array.Empty<UnreadComplement>());
    }

    public static DetectionResult Unrecognized()
    {
        return new DetectionResult(DetectionOutcome.Unrecognized, null, null, null, Array.Empty<UnreadComplement>());
    }
}
