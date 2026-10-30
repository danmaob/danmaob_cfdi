using System;
using System.IO;
using System.Xml.Linq;
using DanmaobCfdi.Core.Reading;
using Xunit;

namespace DanmaobCfdi.Tests.Core.Reading;

public sealed class DocumentDetectorTests
{
    [Fact]
    public void Detect_Cfdi40WithCartaPorte31_ReturnsSupportedWithoutUnreadComplements()
    {
        DetectionResult result = DetectFixture("Deteccion", "cfdi40-cartaporte31.xml");

        Assert.Equal(DetectionOutcome.Supported, result.Outcome);
        Assert.Equal(DocumentKind.Cfdi40, result.Kind);
        Assert.Null(result.Reason);
        Assert.Empty(result.UnreadComplements);
    }

    [Fact]
    public void Detect_ReferenceCfdi_ReturnsSupported()
    {
        DetectionResult result = DetectFixture("Cfdi40", "ingreso-basico.xml");

        Assert.Equal(DetectionOutcome.Supported, result.Outcome);
        Assert.Equal(DocumentKind.Cfdi40, result.Kind);
        Assert.Empty(result.UnreadComplements);
    }

    [Fact]
    public void Detect_ComplementOutsideScope_ReturnsSupportedWithUnreadComplement()
    {
        DetectionResult result = DetectFixture("Cfdi40", "con-extensiones-desconocidas.xml");

        Assert.Equal(DetectionOutcome.Supported, result.Outcome);
        UnreadComplement unread = Assert.Single(result.UnreadComplements);
        Assert.Equal("http://www.sat.gob.mx/ComercioExterior20", unread.NamespaceName);
        Assert.Equal("ComercioExterior", unread.LocalName);
    }

    [Theory]
    [InlineData("cfdi33.xml", UnsupportedReason.CfdiVersion, "3.3")]
    [InlineData("cfdi32.xml", UnsupportedReason.CfdiVersion, "3.2")]
    [InlineData("cartaporte30.xml", UnsupportedReason.ComplementVersion, "CartaPorte 3.0")]
    [InlineData("pagos10.xml", UnsupportedReason.ComplementVersion, "Pagos 1.0")]
    [InlineData("tfd10.xml", UnsupportedReason.ComplementVersion, "TimbreFiscalDigital 1.0")]
    [InlineData("retenciones20.xml", UnsupportedReason.Retenciones, "2.0")]
    public void Detect_UnsupportedFixture_ReturnsUnsupported(string fileName, UnsupportedReason expectedReason, string expectedVersion)
    {
        DetectionResult result = DetectFixture("NoSoportados", fileName);

        Assert.Equal(DetectionOutcome.Unsupported, result.Outcome);
        Assert.Null(result.Kind);
        Assert.Equal(expectedReason, result.Reason);
        Assert.Equal(expectedVersion, result.DetectedVersion);
        Assert.Empty(result.UnreadComplements);
    }

    [Fact]
    public void Detect_CancellationAcknowledgment_ReturnsSupported()
    {
        DetectionResult result = DetectFixture("Deteccion", "acuse-cancelacion.xml");

        Assert.Equal(DetectionOutcome.Supported, result.Outcome);
        Assert.Equal(DocumentKind.CancellationAcknowledgment, result.Kind);
    }

    [Fact]
    public void Detect_CancelacionInOtherNamespace_ReturnsUnrecognized()
    {
        DetectionResult result = DetectFixture("Deteccion", "cancelacion-otro-namespace.xml");

        Assert.Equal(DetectionOutcome.Unrecognized, result.Outcome);
        Assert.Null(result.Kind);
    }

    [Fact]
    public void Detect_UnknownXml_ReturnsUnrecognized()
    {
        DetectionResult result = DetectFixture("Deteccion", "no-reconocido.xml");

        Assert.Equal(DetectionOutcome.Unrecognized, result.Outcome);
        Assert.Null(result.Kind);
        Assert.Null(result.Reason);
    }

    private static DetectionResult DetectFixture(string folder, string fileName)
    {
        byte[] content = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fixtures", folder, fileName));
        SecureXmlLoader loader = new SecureXmlLoader();
        SecureXmlLoadResult result = loader.Load(content);
        XDocument? document = result.Document;
        Assert.NotNull(document);
        DocumentDetector detector = new DocumentDetector();
        return detector.Detect(document);
    }
}
