using System.Xml.Linq;

namespace DanmaobCfdi.Core.Reading.Tfd11;

internal static class Tfd11Extractor
{
    private static readonly XName ComplementoName = XName.Get("Complemento", "http://www.sat.gob.mx/cfd/4");
    private static readonly XName TimbreName = XName.Get("TimbreFiscalDigital", "http://www.sat.gob.mx/TimbreFiscalDigital");

    internal static TimbreFiscalDigitalData? Extract(XElement comprobante)
    {
        XElement? complemento = comprobante.Element(ComplementoName);
        if (complemento is null)
        {
            return null;
        }

        XElement? timbre = complemento.Element(TimbreName);
        if (timbre is null)
        {
            return null;
        }

        return new TimbreFiscalDigitalData
        {
            Version = XmlAttributeReader.Required(timbre, "Version"),
            UUID = XmlAttributeReader.Required(timbre, "UUID"),
            FechaTimbrado = XmlAttributeReader.RequiredDateTime(timbre, "FechaTimbrado"),
            RfcProvCertif = XmlAttributeReader.Required(timbre, "RfcProvCertif"),
            Leyenda = XmlAttributeReader.Optional(timbre, "Leyenda"),
            SelloCFD = XmlAttributeReader.Required(timbre, "SelloCFD"),
            NoCertificadoSAT = XmlAttributeReader.Required(timbre, "NoCertificadoSAT"),
            SelloSAT = XmlAttributeReader.Required(timbre, "SelloSAT")
        };
    }
}
