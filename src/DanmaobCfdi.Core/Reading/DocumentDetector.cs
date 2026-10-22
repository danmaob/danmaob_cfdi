using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DanmaobCfdi.Core.Reading;

internal sealed class DocumentDetector
{
    private const string CfdiNamespace = "http://www.sat.gob.mx/cfd/4";
    private const string CartaPorteNamespace = "http://www.sat.gob.mx/CartaPorte31";
    private const string PagosNamespace = "http://www.sat.gob.mx/Pagos20";
    private const string TimbreNamespace = "http://www.sat.gob.mx/TimbreFiscalDigital";
    private const string ImpuestosLocalesNamespace = "http://www.sat.gob.mx/implocal";
    private const string LeyendasFiscalesNamespace = "http://www.sat.gob.mx/leyendasFiscales";
    private const string CancelacionNamespace = "http://cancelacfd.sat.gob.mx";

    private static readonly XName ComplementoName = XName.Get("Complemento", CfdiNamespace);

    public DetectionResult Detect(XDocument document)
    {
        XElement? root = document.Root;

        if (root is null)
        {
            return DetectionResult.Unrecognized();
        }

        string rootName = root.Name.LocalName;

        if (rootName == "Comprobante")
        {
            return DetectComprobante(root);
        }

        if (rootName == "Retenciones")
        {
            return DetectionResult.Unsupported(UnsupportedReason.Retenciones, ReadVersion(root));
        }

        if (rootName == "Cancelacion" && root.Name.NamespaceName == CancelacionNamespace)
        {
            return DetectionResult.Supported(DocumentKind.CancellationAcknowledgment, Array.Empty<UnreadComplement>());
        }

        return DetectionResult.Unrecognized();
    }

    private static DetectionResult DetectComprobante(XElement root)
    {
        string version = ReadVersion(root);

        if (root.Name.NamespaceName != CfdiNamespace || version != "4.0")
        {
            return DetectionResult.Unsupported(UnsupportedReason.CfdiVersion, version);
        }

        List<UnreadComplement> unread = new List<UnreadComplement>();

        foreach (XElement complemento in root.Elements(ComplementoName))
        {
            foreach (XElement complement in complemento.Elements())
            {
                if (IsUnsupportedComplement(complement))
                {
                    return DetectionResult.Unsupported(UnsupportedReason.ComplementVersion, complement.Name.LocalName + " " + ReadVersion(complement));
                }

                if (!IsReadComplement(complement))
                {
                    unread.Add(new UnreadComplement(complement.Name.NamespaceName, complement.Name.LocalName));
                }
            }
        }

        return DetectionResult.Supported(DocumentKind.Cfdi40, unread);
    }

    private static bool IsUnsupportedComplement(XElement complement)
    {
        string localName = complement.Name.LocalName;
        string namespaceName = complement.Name.NamespaceName;
        string version = ReadVersion(complement);

        if (localName == "CartaPorte")
        {
            return namespaceName != CartaPorteNamespace || version != "3.1";
        }

        if (localName == "Pagos")
        {
            return namespaceName != PagosNamespace || version != "2.0";
        }

        if (localName == "TimbreFiscalDigital")
        {
            return namespaceName != TimbreNamespace || version != "1.1";
        }

        return false;
    }

    private static bool IsReadComplement(XElement complement)
    {
        string namespaceName = complement.Name.NamespaceName;

        return namespaceName == CartaPorteNamespace || namespaceName == PagosNamespace || namespaceName == TimbreNamespace || namespaceName == ImpuestosLocalesNamespace || namespaceName == LeyendasFiscalesNamespace;
    }

    private static string ReadVersion(XElement element)
    {
        XAttribute? attribute = element.Attribute("Version");

        if (attribute is null)
        {
            attribute = element.Attribute("version");
        }

        if (attribute is null)
        {
            return string.Empty;
        }

        return attribute.Value;
    }
}
