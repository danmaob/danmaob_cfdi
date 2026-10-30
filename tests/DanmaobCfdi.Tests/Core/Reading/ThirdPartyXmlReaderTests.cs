using System;
using System.IO;
using DanmaobCfdi.Core;
using DanmaobCfdi.Core.Reading;
using DanmaobCfdi.Core.Reading.Cfdi40;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DanmaobCfdi.Tests.Core.Reading;

public sealed class ThirdPartyXmlReaderTests
{
    private static readonly IThirdPartyXmlReader Reader = CreateReader();

    [Fact]
    public void Read_IngresoFlete_ReturnsReadWithCfdi()
    {
        XmlReadResult result = ReadFixture("Cfdi40", "ingreso-flete.xml");
        Assert.Equal(XmlReadStatus.Read, result.Status);
        Assert.Equal(DocumentKind.Cfdi40, result.Kind);
        ComprobanteData? cfdi = result.Cfdi;
        Assert.NotNull(cfdi);
        Assert.Equal(2688.00m, cfdi.Total);
        Assert.Empty(result.UnreadComplements);
        Assert.Empty(result.StructuralErrors);
        Assert.Null(result.RejectionReason);
    }

    [Fact]
    public void Read_ComplementOutsideScope_ReturnsReadWithUnreadComplement()
    {
        XmlReadResult result = ReadFixture("Cfdi40", "con-extensiones-desconocidas.xml");
        Assert.Equal(XmlReadStatus.Read, result.Status);
        Assert.NotNull(result.Cfdi);
        UnreadComplement unread = Assert.Single(result.UnreadComplements);
        Assert.Equal("ComercioExterior", unread.LocalName);
    }

    [Fact]
    public void Read_AttributeOutsideSchema_ReturnsInvalidWithErrors()
    {
        XmlReadResult result = ReadFixture("Invalidos", "atributo-fuera-del-xsd.xml");
        Assert.Equal(XmlReadStatus.Invalid, result.Status);
        Assert.Null(result.Cfdi);
        StructuralError error = Assert.Single(result.StructuralErrors);
        Assert.Equal(3, error.LineNumber);
    }

    [Fact]
    public void Read_Cfdi33_ReturnsUnsupported()
    {
        XmlReadResult result = ReadFixture("NoSoportados", "cfdi33.xml");
        Assert.Equal(XmlReadStatus.Unsupported, result.Status);
        Assert.Equal(UnsupportedReason.CfdiVersion, result.UnsupportedReason);
        Assert.Equal("3.3", result.DetectedVersion);
        Assert.Null(result.Cfdi);
    }

    [Fact]
    public void Read_UnknownXml_ReturnsUnrecognized()
    {
        XmlReadResult result = ReadFixture("Deteccion", "no-reconocido.xml");
        Assert.Equal(XmlReadStatus.Unrecognized, result.Status);
        Assert.Null(result.Kind);
        Assert.Null(result.Cfdi);
    }

    [Fact]
    public void Read_DocumentWithDtd_ReturnsRejected()
    {
        XmlReadResult result = ReadFixture("Invalidos", "con-dtd.xml");
        Assert.Equal(XmlReadStatus.Rejected, result.Status);
        Assert.Equal(RejectionReason.DtdOrEntity, result.RejectionReason);
        Assert.Null(result.Kind);
    }

    [Fact]
    public void Read_CancellationAcknowledgment_ReturnsReadWithoutModel()
    {
        XmlReadResult result = ReadFixture("Deteccion", "acuse-cancelacion.xml");
        Assert.Equal(XmlReadStatus.Read, result.Status);
        Assert.Equal(DocumentKind.CancellationAcknowledgment, result.Kind);
        Assert.Null(result.Cfdi);
    }

    private static IThirdPartyXmlReader CreateReader()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddDanmaobCfdiCore();
        ServiceProvider provider = services.BuildServiceProvider();
        return provider.GetRequiredService<IThirdPartyXmlReader>();
    }

    private static XmlReadResult ReadFixture(string folder, string fileName)
    {
        byte[] content = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fixtures", folder, fileName));
        return Reader.Read(content);
    }
}
