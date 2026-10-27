using System;
using System.Collections.Generic;
using System.Xml.Linq;
using DanmaobCfdi.Core.Reading.Pagos20;
using DanmaobCfdi.Core.Reading.Tfd11;

namespace DanmaobCfdi.Core.Reading.Cfdi40;

internal sealed class Cfdi40Extractor
{
    private static readonly XNamespace Cfdi = XNamespace.Get("http://www.sat.gob.mx/cfd/4");

    public ComprobanteData Extract(XDocument document)
    {
        XElement? root = document.Root;

        if (root is null)
        {
            throw new ArgumentException("The document has no root element.", nameof(document));
        }

        return new ComprobanteData
        {
            Version = XmlAttributeReader.Required(root, "Version"),
            Serie = XmlAttributeReader.Optional(root, "Serie"),
            Folio = XmlAttributeReader.Optional(root, "Folio"),
            Fecha = XmlAttributeReader.RequiredDateTime(root, "Fecha"),
            FormaPago = XmlAttributeReader.Optional(root, "FormaPago"),
            CondicionesDePago = XmlAttributeReader.Optional(root, "CondicionesDePago"),
            SubTotal = XmlAttributeReader.RequiredDecimal(root, "SubTotal"),
            Descuento = XmlAttributeReader.OptionalDecimal(root, "Descuento"),
            Moneda = XmlAttributeReader.Required(root, "Moneda"),
            TipoCambio = XmlAttributeReader.OptionalDecimal(root, "TipoCambio"),
            Total = XmlAttributeReader.RequiredDecimal(root, "Total"),
            TipoDeComprobante = XmlAttributeReader.Required(root, "TipoDeComprobante"),
            Exportacion = XmlAttributeReader.Required(root, "Exportacion"),
            MetodoPago = XmlAttributeReader.Optional(root, "MetodoPago"),
            LugarExpedicion = XmlAttributeReader.Required(root, "LugarExpedicion"),
            CfdiRelacionados = ReadCfdiRelacionados(root),
            Emisor = ReadEmisor(RequiredChild(root, "Emisor")),
            Receptor = ReadReceptor(RequiredChild(root, "Receptor")),
            Conceptos = ReadConceptos(RequiredChild(root, "Conceptos")),
            Impuestos = Cfdi40ImpuestosReader.ReadImpuestos(root),
            TimbreFiscalDigital = Tfd11Extractor.Extract(root),
            Pagos = Pagos20Extractor.Extract(root)
        };
    }

    private static XElement RequiredChild(XElement parent, string localName)
    {
        XElement? child = parent.Element(Cfdi + localName);

        if (child is null)
        {
            throw new ArgumentException("A required CFDI element is missing: " + localName, nameof(parent));
        }

        return child;
    }

    private static IReadOnlyList<CfdiRelacionadosData> ReadCfdiRelacionados(XElement root)
    {
        List<CfdiRelacionadosData> grupos = new List<CfdiRelacionadosData>();

        foreach (XElement grupo in root.Elements(Cfdi + "CfdiRelacionados"))
        {
            List<string> uuids = new List<string>();

            foreach (XElement relacionado in grupo.Elements(Cfdi + "CfdiRelacionado"))
            {
                uuids.Add(XmlAttributeReader.Required(relacionado, "UUID"));
            }

            grupos.Add(new CfdiRelacionadosData
            {
                TipoRelacion = XmlAttributeReader.Required(grupo, "TipoRelacion"),
                Uuids = uuids
            });
        }

        return grupos;
    }

    private static EmisorData ReadEmisor(XElement emisor)
    {
        return new EmisorData
        {
            Rfc = XmlAttributeReader.Required(emisor, "Rfc"),
            Nombre = XmlAttributeReader.Required(emisor, "Nombre"),
            RegimenFiscal = XmlAttributeReader.Required(emisor, "RegimenFiscal"),
            FacAtrAdquirente = XmlAttributeReader.Optional(emisor, "FacAtrAdquirente")
        };
    }

    private static ReceptorData ReadReceptor(XElement receptor)
    {
        return new ReceptorData
        {
            Rfc = XmlAttributeReader.Required(receptor, "Rfc"),
            Nombre = XmlAttributeReader.Required(receptor, "Nombre"),
            DomicilioFiscalReceptor = XmlAttributeReader.Required(receptor, "DomicilioFiscalReceptor"),
            ResidenciaFiscal = XmlAttributeReader.Optional(receptor, "ResidenciaFiscal"),
            NumRegIdTrib = XmlAttributeReader.Optional(receptor, "NumRegIdTrib"),
            RegimenFiscalReceptor = XmlAttributeReader.Required(receptor, "RegimenFiscalReceptor"),
            UsoCFDI = XmlAttributeReader.Required(receptor, "UsoCFDI")
        };
    }

    private static IReadOnlyList<ConceptoData> ReadConceptos(XElement conceptos)
    {
        List<ConceptoData> lista = new List<ConceptoData>();

        foreach (XElement concepto in conceptos.Elements(Cfdi + "Concepto"))
        {
            XElement? impuestos = concepto.Element(Cfdi + "Impuestos");

            lista.Add(new ConceptoData
            {
                ClaveProdServ = XmlAttributeReader.Required(concepto, "ClaveProdServ"),
                NoIdentificacion = XmlAttributeReader.Optional(concepto, "NoIdentificacion"),
                Cantidad = XmlAttributeReader.RequiredDecimal(concepto, "Cantidad"),
                ClaveUnidad = XmlAttributeReader.Required(concepto, "ClaveUnidad"),
                Unidad = XmlAttributeReader.Optional(concepto, "Unidad"),
                Descripcion = XmlAttributeReader.Required(concepto, "Descripcion"),
                ValorUnitario = XmlAttributeReader.RequiredDecimal(concepto, "ValorUnitario"),
                Importe = XmlAttributeReader.RequiredDecimal(concepto, "Importe"),
                Descuento = XmlAttributeReader.OptionalDecimal(concepto, "Descuento"),
                ObjetoImp = XmlAttributeReader.Required(concepto, "ObjetoImp"),
                Traslados = Cfdi40ImpuestosReader.ReadTraslados(impuestos),
                Retenciones = Cfdi40ImpuestosReader.ReadRetenciones(impuestos)
            });
        }

        return lista;
    }
}
