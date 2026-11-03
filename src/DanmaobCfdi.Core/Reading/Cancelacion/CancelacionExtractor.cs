using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace DanmaobCfdi.Core.Reading.Cancelacion;

internal static class CancelacionExtractor
{
    private const string FechaFormat = "yyyy-MM-dd'T'HH:mm:ss";
    private static readonly XNamespace AcuseNs = XNamespace.Get("http://cancelacfd.sat.gob.mx");

    internal static CancelacionData Extract(XDocument document)
    {
        XElement? root = document.Root;
        if (root is null)
        {
            return new CancelacionData();
        }

        List<FolioData> folios = new List<FolioData>();
        foreach (XElement folio in root.Elements(AcuseNs + "Folios"))
        {
            folios.Add(ReadFolio(folio));
        }

        return new CancelacionData
        {
            RfcEmisor = Attribute(root, "RfcEmisor"),
            Fecha = ParseFecha(Attribute(root, "Fecha")),
            Folios = folios
        };
    }

    private static FolioData ReadFolio(XElement folio)
    {
        return new FolioData
        {
            UUID = Child(folio, "UUID") ?? string.Empty,
            UUIDEstatus = Child(folio, "UUIDEstatus") ?? string.Empty,
            UUIDdescripcion = Child(folio, "UUIDdescripcion"),
            UUIDfecha = ParseOptionalFecha(Child(folio, "UUIDfecha"))
        };
    }

    private static string Attribute(XElement element, string name)
    {
        XAttribute? attribute = element.Attribute(name);
        if (attribute is null)
        {
            return string.Empty;
        }

        return attribute.Value.Trim();
    }

    private static string? Child(XElement parent, string name)
    {
        XElement? child = parent.Element(AcuseNs + name);
        if (child is null)
        {
            return null;
        }

        return child.Value.Trim();
    }

    private static DateTime ParseFecha(string value)
    {
        return DateTime.ParseExact(value, FechaFormat, CultureInfo.InvariantCulture);
    }

    private static DateTime? ParseOptionalFecha(string? value)
    {
        if (value is null)
        {
            return null;
        }

        return ParseFecha(value);
    }
}
