using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Xsl;
using DanmaobCfdi.Core.Resources;
using Xunit;

namespace DanmaobCfdi.Tests.Core.Resources;

public sealed class SatCompiledResourcesTests
{
    private static readonly SatCompiledResources Shared = new SatCompiledResources(new SatResourceResolver());

    [Fact]
    public void CfdiOriginalStringTransform_RequestedTwice_CompiledOnce()
    {
        XslCompiledTransform first = Shared.CfdiOriginalStringTransform;
        XslCompiledTransform second = Shared.CfdiOriginalStringTransform;
        Assert.Same(first, second);
    }

    [Fact]
    public void RetencionesOriginalStringTransform_RequestedTwice_CompiledOnce()
    {
        XslCompiledTransform first = Shared.RetencionesOriginalStringTransform;
        XslCompiledTransform second = Shared.RetencionesOriginalStringTransform;
        Assert.Same(first, second);
    }

    [Fact]
    public void CfdiSchemas_RequestedTwice_CompiledOnceWithSupportedNamespaces()
    {
        XmlSchemaSet first = Shared.CfdiSchemas;
        XmlSchemaSet second = Shared.CfdiSchemas;
        Assert.Same(first, second);
        Assert.True(first.IsCompiled);
        Assert.True(first.Contains("http://www.sat.gob.mx/cfd/4"));
        Assert.True(first.Contains("http://www.sat.gob.mx/Pagos20"));
        Assert.True(first.Contains("http://www.sat.gob.mx/CartaPorte31"));
        Assert.True(first.Contains("http://www.sat.gob.mx/TimbreFiscalDigital"));
        Assert.True(first.Contains("http://www.sat.gob.mx/implocal"));
        Assert.True(first.Contains("http://www.sat.gob.mx/leyendasFiscales"));
    }

    [Fact]
    public void RetencionesSchemas_RequestedTwice_CompiledOnceWithRetencionesNamespace()
    {
        XmlSchemaSet first = Shared.RetencionesSchemas;
        XmlSchemaSet second = Shared.RetencionesSchemas;
        Assert.Same(first, second);
        Assert.True(first.IsCompiled);
        Assert.True(first.Contains("http://www.sat.gob.mx/esquemas/retencionpago/2"));
    }

    [Fact]
    public void CfdiOriginalStringTransform_ReferenceCfdi_MatchesXsltproc()
    {
        string folder = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Cfdi40");
        string expected = File.ReadAllText(Path.Combine(folder, "ingreso-basico.cadena.txt"), Encoding.UTF8);
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.DtdProcessing = DtdProcessing.Prohibit;
        using XmlReader reader = XmlReader.Create(Path.Combine(folder, "ingreso-basico.xml"), settings);
        using StringWriter writer = new StringWriter();
        Shared.CfdiOriginalStringTransform.Transform(reader, null, writer);
        Assert.Equal(expected, writer.ToString());
    }
}
