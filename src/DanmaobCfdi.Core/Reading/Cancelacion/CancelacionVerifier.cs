using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace DanmaobCfdi.Core.Reading.Cancelacion;

internal static class CancelacionVerifier
{
    private const string FechaFormat = "yyyy-MM-dd'T'HH:mm:ss";
    private static readonly XNamespace AcuseNs = XNamespace.Get("http://cancelacfd.sat.gob.mx");

    internal static IReadOnlyList<StructuralError> Verify(XDocument document)
    {
        List<StructuralError> errors = new List<StructuralError>();
        XElement? root = document.Root;
        if (root is null)
        {
            errors.Add(new StructuralError(0, 0, "El acuse no tiene elemento raiz."));
            return errors;
        }

        if (Attribute(root, "RfcEmisor").Length == 0)
        {
            AddError(errors, root, "Falta el atributo RfcEmisor del acuse.");
        }

        if (!IsFecha(Attribute(root, "Fecha")))
        {
            AddError(errors, root, "Falta el atributo Fecha del acuse o no tiene el formato aaaa-mm-ddThh:mm:ss.");
        }

        bool hasFolios = false;
        foreach (XElement folio in root.Elements(AcuseNs + "Folios"))
        {
            hasFolios = true;
            VerifyFolio(folio, errors);
        }

        if (!hasFolios)
        {
            AddError(errors, root, "El acuse no tiene elementos Folios.");
        }

        return errors;
    }

    private static void VerifyFolio(XElement folio, List<StructuralError> errors)
    {
        XElement? uuid = folio.Element(AcuseNs + "UUID");
        if (uuid is null || !Guid.TryParseExact(uuid.Value.Trim(), "D", out Guid _))
        {
            AddError(errors, uuid ?? folio, "Falta el UUID de un folio o no tiene el formato de un UUID.");
        }

        XElement? estatus = folio.Element(AcuseNs + "UUIDEstatus");
        if (estatus is null || estatus.Value.Trim().Length == 0)
        {
            AddError(errors, estatus ?? folio, "Falta el UUIDEstatus de un folio.");
        }

        XElement? fecha = folio.Element(AcuseNs + "UUIDfecha");
        if (fecha is not null && !IsFecha(fecha.Value.Trim()))
        {
            AddError(errors, fecha, "El UUIDfecha de un folio no tiene el formato aaaa-mm-ddThh:mm:ss.");
        }
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

    private static bool IsFecha(string value)
    {
        return DateTime.TryParseExact(value, FechaFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _);
    }

    private static void AddError(List<StructuralError> errors, XElement element, string message)
    {
        IXmlLineInfo info = element;
        errors.Add(new StructuralError(info.LineNumber, info.LinePosition, message));
    }
}
