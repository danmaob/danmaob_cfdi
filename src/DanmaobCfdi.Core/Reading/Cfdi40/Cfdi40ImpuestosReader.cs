using System.Collections.Generic;
using System.Xml.Linq;

namespace DanmaobCfdi.Core.Reading.Cfdi40;

internal static class Cfdi40ImpuestosReader
{
    private static readonly XNamespace Cfdi = XNamespace.Get("http://www.sat.gob.mx/cfd/4");

    internal static IReadOnlyList<TrasladoData> ReadTraslados(XElement? impuestos)
    {
        List<TrasladoData> traslados = new List<TrasladoData>();

        if (impuestos is null)
        {
            return traslados;
        }

        foreach (XElement traslado in impuestos.Elements(Cfdi + "Traslados").Elements(Cfdi + "Traslado"))
        {
            traslados.Add(new TrasladoData
            {
                Base = XmlAttributeReader.RequiredDecimal(traslado, "Base"),
                Impuesto = XmlAttributeReader.Required(traslado, "Impuesto"),
                TipoFactor = XmlAttributeReader.Required(traslado, "TipoFactor"),
                TasaOCuota = XmlAttributeReader.OptionalDecimal(traslado, "TasaOCuota"),
                Importe = XmlAttributeReader.OptionalDecimal(traslado, "Importe")
            });
        }

        return traslados;
    }

    internal static IReadOnlyList<RetencionData> ReadRetenciones(XElement? impuestos)
    {
        List<RetencionData> retenciones = new List<RetencionData>();

        if (impuestos is null)
        {
            return retenciones;
        }

        foreach (XElement retencion in impuestos.Elements(Cfdi + "Retenciones").Elements(Cfdi + "Retencion"))
        {
            retenciones.Add(new RetencionData
            {
                Base = XmlAttributeReader.OptionalDecimal(retencion, "Base"),
                Impuesto = XmlAttributeReader.Required(retencion, "Impuesto"),
                TipoFactor = XmlAttributeReader.Optional(retencion, "TipoFactor"),
                TasaOCuota = XmlAttributeReader.OptionalDecimal(retencion, "TasaOCuota"),
                Importe = XmlAttributeReader.RequiredDecimal(retencion, "Importe")
            });
        }

        return retenciones;
    }

    internal static ImpuestosData? ReadImpuestos(XElement comprobante)
    {
        XElement? impuestos = comprobante.Element(Cfdi + "Impuestos");

        if (impuestos is null)
        {
            return null;
        }

        return new ImpuestosData
        {
            TotalImpuestosRetenidos = XmlAttributeReader.OptionalDecimal(impuestos, "TotalImpuestosRetenidos"),
            TotalImpuestosTrasladados = XmlAttributeReader.OptionalDecimal(impuestos, "TotalImpuestosTrasladados"),
            Retenciones = ReadRetenciones(impuestos),
            Traslados = ReadTraslados(impuestos)
        };
    }
}
