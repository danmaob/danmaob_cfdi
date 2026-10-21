using System.Xml.Linq;

namespace DanmaobCfdi.Core.Reading;

internal sealed class SecureXmlLoadResult
{
    private SecureXmlLoadResult(XDocument? document, RejectionReason? reason)
    {
        Document = document;
        Reason = reason;
    }

    public XDocument? Document { get; }

    public RejectionReason? Reason { get; }

    public bool IsLoaded
    {
        get
        {
            return Document is not null;
        }
    }

    public static SecureXmlLoadResult Loaded(XDocument document)
    {
        return new SecureXmlLoadResult(document, null);
    }

    public static SecureXmlLoadResult Rejected(RejectionReason reason)
    {
        return new SecureXmlLoadResult(null, reason);
    }
}
