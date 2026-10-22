using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using DanmaobCfdi.Core.Resources;

namespace DanmaobCfdi.Core.Reading;

internal sealed class XmlStructuralValidator
{
    internal const int MaxErrors = 100;

    private static readonly XName ComplementoName = XName.Get("Complemento", "http://www.sat.gob.mx/cfd/4");
    private static readonly XName ComplementoConceptoName = XName.Get("ComplementoConcepto", "http://www.sat.gob.mx/cfd/4");
    private static readonly XName AddendaName = XName.Get("Addenda", "http://www.sat.gob.mx/cfd/4");

    private readonly SatCompiledResources _resources;

    public XmlStructuralValidator(SatCompiledResources resources)
    {
        _resources = resources;
    }

    public IReadOnlyList<StructuralError> Validate(XDocument document)
    {
        XmlSchemaSet schemas = _resources.CfdiSchemas;
        List<StructuralError> errors = new List<StructuralError>();
        document.Validate(schemas, (sender, args) => Collect(sender, args, schemas, errors));
        return errors;
    }

    private static void Collect(object? sender, ValidationEventArgs args, XmlSchemaSet schemas, List<StructuralError> errors)
    {
        if (args.Severity != XmlSeverityType.Error)
        {
            return;
        }
        if (errors.Count >= MaxErrors)
        {
            return;
        }
        XObject? node = sender as XObject;
        if (IsInsideUnknownExtension(node, schemas))
        {
            return;
        }
        int lineNumber = 0;
        int linePosition = 0;
        IXmlLineInfo? lineInfo = node;
        if (lineInfo is not null && lineInfo.HasLineInfo())
        {
            lineNumber = lineInfo.LineNumber;
            linePosition = lineInfo.LinePosition;
        }
        errors.Add(new StructuralError(lineNumber, linePosition, args.Message));
    }

    private static bool IsInsideUnknownExtension(XObject? node, XmlSchemaSet schemas)
    {
        XElement? element = node as XElement;
        if (element is null && node is not null)
        {
            element = node.Parent;
        }
        while (element is not null)
        {
            XElement? parent = element.Parent;
            if (parent is not null && IsExtensionPoint(parent.Name) && !schemas.Contains(element.Name.NamespaceName))
            {
                return true;
            }
            element = parent;
        }
        return false;
    }

    private static bool IsExtensionPoint(XName name)
    {
        return name == ComplementoName || name == ComplementoConceptoName || name == AddendaName;
    }
}
