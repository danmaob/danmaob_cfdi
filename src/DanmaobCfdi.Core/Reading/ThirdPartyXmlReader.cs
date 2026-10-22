using System;
using System.Collections.Generic;
using System.Xml.Linq;
using DanmaobCfdi.Core.Reading.Cfdi40;

namespace DanmaobCfdi.Core.Reading;

internal sealed class ThirdPartyXmlReader : IThirdPartyXmlReader
{
    private readonly SecureXmlLoader _loader;
    private readonly DocumentDetector _detector;
    private readonly XmlStructuralValidator _validator;
    private readonly Cfdi40Extractor _extractor;

    public ThirdPartyXmlReader(SecureXmlLoader loader, DocumentDetector detector, XmlStructuralValidator validator, Cfdi40Extractor extractor)
    {
        _loader = loader;
        _detector = detector;
        _validator = validator;
        _extractor = extractor;
    }

    public XmlReadResult Read(byte[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        SecureXmlLoadResult loaded = _loader.Load(content);
        XDocument? document = loaded.Document;

        if (document is null)
        {
            return new XmlReadResult
            {
                Status = XmlReadStatus.Rejected,
                RejectionReason = loaded.Reason
            };
        }

        DetectionResult detection = _detector.Detect(document);

        if (detection.Outcome == DetectionOutcome.Unrecognized)
        {
            return new XmlReadResult
            {
                Status = XmlReadStatus.Unrecognized
            };
        }

        if (detection.Outcome == DetectionOutcome.Unsupported)
        {
            return new XmlReadResult
            {
                Status = XmlReadStatus.Unsupported,
                UnsupportedReason = detection.Reason,
                DetectedVersion = detection.DetectedVersion
            };
        }

        if (detection.Kind != DocumentKind.Cfdi40)
        {
            return new XmlReadResult
            {
                Status = XmlReadStatus.Read,
                Kind = detection.Kind
            };
        }

        IReadOnlyList<StructuralError> errors = _validator.Validate(document);

        if (errors.Count > 0)
        {
            return new XmlReadResult
            {
                Status = XmlReadStatus.Invalid,
                Kind = detection.Kind,
                StructuralErrors = errors
            };
        }

        return new XmlReadResult
        {
            Status = XmlReadStatus.Read,
            Kind = detection.Kind,
            Cfdi = _extractor.Extract(document),
            UnreadComplements = detection.UnreadComplements
        };
    }
}
