using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace DanmaobCfdi.Core.Reading.Pagos20;

internal static class Pagos20Extractor
{
    private static readonly XName ComplementoName = XName.Get("Complemento", "http://www.sat.gob.mx/cfd/4");
    private static readonly XNamespace Pago20 = XNamespace.Get("http://www.sat.gob.mx/Pagos20");

    internal static PagosData? Extract(XElement comprobante)
    {
        XElement? complemento = comprobante.Element(ComplementoName);
        if (complemento is null)
        {
            return null;
        }

        XElement? pagos = complemento.Element(Pago20 + "Pagos");
        if (pagos is null)
        {
            return null;
        }

        List<PagoData> lista = new List<PagoData>();
        foreach (XElement pago in pagos.Elements(Pago20 + "Pago"))
        {
            lista.Add(ReadPago(pago));
        }

        return new PagosData
        {
            Version = XmlAttributeReader.Required(pagos, "Version"),
            Totales = ReadTotales(pagos.Element(Pago20 + "Totales")),
            Pagos = lista
        };
    }

    private static TotalesData ReadTotales(XElement? totales)
    {
        if (totales is null)
        {
            return new TotalesData();
        }

        return new TotalesData
        {
            TotalRetencionesIVA = XmlAttributeReader.OptionalDecimal(totales, "TotalRetencionesIVA"),
            TotalRetencionesISR = XmlAttributeReader.OptionalDecimal(totales, "TotalRetencionesISR"),
            TotalRetencionesIEPS = XmlAttributeReader.OptionalDecimal(totales, "TotalRetencionesIEPS"),
            TotalTrasladosBaseIVA16 = XmlAttributeReader.OptionalDecimal(totales, "TotalTrasladosBaseIVA16"),
            TotalTrasladosImpuestoIVA16 = XmlAttributeReader.OptionalDecimal(totales, "TotalTrasladosImpuestoIVA16"),
            TotalTrasladosBaseIVA8 = XmlAttributeReader.OptionalDecimal(totales, "TotalTrasladosBaseIVA8"),
            TotalTrasladosImpuestoIVA8 = XmlAttributeReader.OptionalDecimal(totales, "TotalTrasladosImpuestoIVA8"),
            TotalTrasladosBaseIVA0 = XmlAttributeReader.OptionalDecimal(totales, "TotalTrasladosBaseIVA0"),
            TotalTrasladosImpuestoIVA0 = XmlAttributeReader.OptionalDecimal(totales, "TotalTrasladosImpuestoIVA0"),
            TotalTrasladosBaseIVAExento = XmlAttributeReader.OptionalDecimal(totales, "TotalTrasladosBaseIVAExento"),
            MontoTotalPagos = XmlAttributeReader.RequiredDecimal(totales, "MontoTotalPagos")
        };
    }

    private static PagoData ReadPago(XElement pago)
    {
        List<DoctoRelacionadoData> doctos = new List<DoctoRelacionadoData>();
        foreach (XElement docto in pago.Elements(Pago20 + "DoctoRelacionado"))
        {
            doctos.Add(ReadDoctoRelacionado(docto));
        }

        return new PagoData
        {
            FechaPago = XmlAttributeReader.RequiredDateTime(pago, "FechaPago"),
            FormaDePagoP = XmlAttributeReader.Required(pago, "FormaDePagoP"),
            MonedaP = XmlAttributeReader.Required(pago, "MonedaP"),
            TipoCambioP = XmlAttributeReader.OptionalDecimal(pago, "TipoCambioP"),
            Monto = XmlAttributeReader.RequiredDecimal(pago, "Monto"),
            NumOperacion = XmlAttributeReader.Optional(pago, "NumOperacion"),
            DoctosRelacionados = doctos
        };
    }

    private static DoctoRelacionadoData ReadDoctoRelacionado(XElement docto)
    {
        return new DoctoRelacionadoData
        {
            IdDocumento = XmlAttributeReader.Required(docto, "IdDocumento"),
            Serie = XmlAttributeReader.Optional(docto, "Serie"),
            Folio = XmlAttributeReader.Optional(docto, "Folio"),
            MonedaDR = XmlAttributeReader.Required(docto, "MonedaDR"),
            EquivalenciaDR = XmlAttributeReader.OptionalDecimal(docto, "EquivalenciaDR"),
            NumParcialidad = int.Parse(XmlAttributeReader.Required(docto, "NumParcialidad"), CultureInfo.InvariantCulture),
            ImpSaldoAnt = XmlAttributeReader.RequiredDecimal(docto, "ImpSaldoAnt"),
            ImpPagado = XmlAttributeReader.RequiredDecimal(docto, "ImpPagado"),
            ImpSaldoInsoluto = XmlAttributeReader.RequiredDecimal(docto, "ImpSaldoInsoluto"),
            ObjetoImpDR = XmlAttributeReader.Required(docto, "ObjetoImpDR")
        };
    }
}
