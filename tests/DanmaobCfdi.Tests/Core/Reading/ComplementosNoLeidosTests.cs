using System;
using System.IO;
using System.Xml.Linq;
using DanmaobCfdi.Core;
using DanmaobCfdi.Core.Reading;
using DanmaobCfdi.Core.Reading.Cfdi40;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DanmaobCfdi.Tests.Core.Reading;

public sealed class ComplementosNoLeidosTests
{
    [Fact]
    public void Detect_ImpuestosLocalesAndLeyendas_ReportsBothAsUnread()
    {
        byte[] content = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fixtures", "Cfdi40", "ingreso-implocal-leyendas.xml"));
        SecureXmlLoader loader = new SecureXmlLoader();
        SecureXmlLoadResult loaded = loader.Load(content);
        XDocument? document = loaded.Document;
        Assert.NotNull(document);
        DocumentDetector detector = new DocumentDetector();
        DetectionResult result = detector.Detect(document);
        Assert.Equal(DetectionOutcome.Supported, result.Outcome);
        Assert.Equal(2, result.UnreadComplements.Count);
        Assert.Equal("http://www.sat.gob.mx/implocal", result.UnreadComplements[0].NamespaceName);
        Assert.Equal("ImpuestosLocales", result.UnreadComplements[0].LocalName);
        Assert.Equal("http://www.sat.gob.mx/leyendasFiscales", result.UnreadComplements[1].NamespaceName);
        Assert.Equal("LeyendasFiscales", result.UnreadComplements[1].LocalName);
    }

    [Fact]
    public void Read_ImpuestosLocalesAndLeyendas_ReturnsReadWithBothUnread()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddDanmaobCfdiCore();
        ServiceProvider provider = services.BuildServiceProvider();
        IThirdPartyXmlReader reader = provider.GetRequiredService<IThirdPartyXmlReader>();
        byte[] content = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fixtures", "Cfdi40", "ingreso-implocal-leyendas.xml"));
        XmlReadResult result = reader.Read(content);
        Assert.Equal(XmlReadStatus.Read, result.Status);
        ComprobanteData? cfdi = result.Cfdi;
        Assert.NotNull(cfdi);
        Assert.NotNull(cfdi.TimbreFiscalDigital);
        Assert.Equal(2, result.UnreadComplements.Count);
    }
}
