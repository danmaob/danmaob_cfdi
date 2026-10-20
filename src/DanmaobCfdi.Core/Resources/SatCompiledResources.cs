using System;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Xsl;

namespace DanmaobCfdi.Core.Resources;

internal sealed class SatCompiledResources
{
    private const string CfdiStylesheetAddress = "http://www.sat.gob.mx/sitio_internet/cfd/4/cadenaoriginal_4_0/cadenaoriginal_4_0.xslt";
    private const string RetencionesStylesheetAddress = "http://www.sat.gob.mx/esquemas/retencionpago/2/retenciones.xslt";
    private const string RetencionesSchemaAddress = "http://www.sat.gob.mx/esquemas/retencionpago/2/retencionpagov2.xsd";
    private static readonly string[] CfdiSchemaAddresses = new string[] { "http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd", "http://www.sat.gob.mx/sitio_internet/cfd/Pagos/Pagos20.xsd", "http://www.sat.gob.mx/sitio_internet/cfd/CartaPorte/CartaPorte31.xsd", "http://www.sat.gob.mx/sitio_internet/cfd/implocal/implocal.xsd", "http://www.sat.gob.mx/sitio_internet/cfd/leyendasFiscales/leyendasFisc.xsd", "http://www.sat.gob.mx/sitio_internet/cfd/TimbreFiscalDigital/TimbreFiscalDigitalv11.xsd" };

    private readonly SatResourceResolver _resolver;
    private readonly Lazy<XslCompiledTransform> _cfdiTransform;
    private readonly Lazy<XslCompiledTransform> _retencionesTransform;
    private readonly Lazy<XmlSchemaSet> _cfdiSchemas;
    private readonly Lazy<XmlSchemaSet> _retencionesSchemas;

    public SatCompiledResources(SatResourceResolver resolver)
    {
        _resolver = resolver;
        _cfdiTransform = new Lazy<XslCompiledTransform>(LoadCfdiTransform, LazyThreadSafetyMode.ExecutionAndPublication);
        _retencionesTransform = new Lazy<XslCompiledTransform>(LoadRetencionesTransform, LazyThreadSafetyMode.ExecutionAndPublication);
        _cfdiSchemas = new Lazy<XmlSchemaSet>(LoadCfdiSchemas, LazyThreadSafetyMode.ExecutionAndPublication);
        _retencionesSchemas = new Lazy<XmlSchemaSet>(LoadRetencionesSchemas, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public XslCompiledTransform CfdiOriginalStringTransform
    {
        get
        {
            return _cfdiTransform.Value;
        }
    }

    public XslCompiledTransform RetencionesOriginalStringTransform
    {
        get
        {
            return _retencionesTransform.Value;
        }
    }

    public XmlSchemaSet CfdiSchemas
    {
        get
        {
            return _cfdiSchemas.Value;
        }
    }

    public XmlSchemaSet RetencionesSchemas
    {
        get
        {
            return _retencionesSchemas.Value;
        }
    }

    private XslCompiledTransform LoadCfdiTransform()
    {
        return LoadTransform(CfdiStylesheetAddress);
    }

    private XslCompiledTransform LoadRetencionesTransform()
    {
        return LoadTransform(RetencionesStylesheetAddress);
    }

    private XmlSchemaSet LoadCfdiSchemas()
    {
        return LoadSchemas(CfdiSchemaAddresses);
    }

    private XmlSchemaSet LoadRetencionesSchemas()
    {
        return LoadSchemas(new string[] { RetencionesSchemaAddress });
    }

    private XslCompiledTransform LoadTransform(string address)
    {
        XslCompiledTransform transform = new XslCompiledTransform(false);
        using (XmlReader reader = CreateReader(address))
        {
            transform.Load(reader, XsltSettings.Default, _resolver);
        }
        return transform;
    }

    private XmlSchemaSet LoadSchemas(string[] addresses)
    {
        XmlSchemaSet schemas = new XmlSchemaSet();
        schemas.XmlResolver = _resolver;
        foreach (string address in addresses)
        {
            using (XmlReader reader = CreateReader(address))
            {
                schemas.Add(null, reader);
            }
        }
        schemas.Compile();
        return schemas;
    }

    private XmlReader CreateReader(string address)
    {
        if (_resolver.GetEntity(new Uri(address), null, typeof(Stream)) is not Stream stream)
        {
            throw new InvalidOperationException("The SAT resource could not be opened: " + address);
        }
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.DtdProcessing = DtdProcessing.Prohibit;
        settings.CloseInput = true;
        return XmlReader.Create(stream, settings, address);
    }
}
