using System;
using System.IO;
using System.Xml.Linq;
using DanmaobCfdi.Core;
using DanmaobCfdi.Core.Reading;
using DanmaobCfdi.Core.Reading.Cfdi40;
using DanmaobCfdi.Core.Reading.Tfd11;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DanmaobCfdi.Tests.Core.Reading.Tfd11;

public sealed class Tfd11ExtractorTests
{
    [Fact]
    public void Extract_IngresoTimbrado_ReadsAllAttributes()
    {
        XElement root = LoadRoot("ingreso-timbrado.xml");
        TimbreFiscalDigitalData? timbre = Tfd11Extractor.Extract(root);
        Assert.NotNull(timbre);
        Assert.Equal("1.1", timbre.Version);
        Assert.Equal("E3B5F1A2-7C4D-4E8F-9A1B-2C3D4E5F6A7B", timbre.UUID);
        Assert.Equal("SPR190613I52", timbre.RfcProvCertif);
        Assert.Null(timbre.Leyenda);
        Assert.Equal(XmlAttributeReader.Required(root, "Sello"), timbre.SelloCFD);
        Assert.Equal("30001000000500003456", timbre.NoCertificadoSAT);
        Assert.Equal("U0VMTE8gU0FUIERFIFBSVUVCQVMgUEFSQSBEQU5NQU9CIENGREk=", timbre.SelloSAT);
    }

    [Fact]
    public void Extract_IngresoTimbrado_ReadsFechaTimbradoWithoutTimeZone()
    {
        XElement root = LoadRoot("ingreso-timbrado.xml");
        TimbreFiscalDigitalData? timbre = Tfd11Extractor.Extract(root);
        Assert.NotNull(timbre);
        Assert.Equal(new DateTime(2026, 10, 16, 11, 31, 5), timbre.FechaTimbrado);
        Assert.Equal(DateTimeKind.Unspecified, timbre.FechaTimbrado.Kind);
    }

    [Fact]
    public void Extract_CfdiWithoutTimbre_ReturnsNull()
    {
        XElement root = LoadRoot("ingreso-flete.xml");
        TimbreFiscalDigitalData? timbre = Tfd11Extractor.Extract(root);
        Assert.Null(timbre);
    }

    [Fact]
    public void Read_IngresoTimbrado_ReturnsTimbreInsideCfdi()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddDanmaobCfdiCore();
        ServiceProvider provider = services.BuildServiceProvider();
        IThirdPartyXmlReader reader = provider.GetRequiredService<IThirdPartyXmlReader>();
        byte[] content = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fixtures", "Cfdi40", "ingreso-timbrado.xml"));
        XmlReadResult result = reader.Read(content);
        Assert.Equal(XmlReadStatus.Read, result.Status);
        ComprobanteData? cfdi = result.Cfdi;
        Assert.NotNull(cfdi);
        TimbreFiscalDigitalData? timbre = cfdi.TimbreFiscalDigital;
        Assert.NotNull(timbre);
        Assert.Equal("E3B5F1A2-7C4D-4E8F-9A1B-2C3D4E5F6A7B", timbre.UUID);
    }

    private static XElement LoadRoot(string fileName)
    {
        byte[] content = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fixtures", "Cfdi40", fileName));
        SecureXmlLoader loader = new SecureXmlLoader();
        SecureXmlLoadResult result = loader.Load(content);
        XDocument? document = result.Document;
        Assert.NotNull(document);
        XElement? root = document.Root;
        Assert.NotNull(root);
        return root;
    }
}
