using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using DanmaobCfdi.Core;
using DanmaobCfdi.Core.Reading;
using DanmaobCfdi.Core.Reading.Cancelacion;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DanmaobCfdi.Tests.Core.Reading.Cancelacion;

public sealed class CancelacionReadingTests
{
    [Fact]
    public void Read_AcusePax_ReturnsReadWithTrimmedValues()
    {
        XmlReadResult result = ReadWithReader("acuse-pax.xml");
        Assert.Equal(XmlReadStatus.Read, result.Status);
        Assert.Equal(DocumentKind.CancellationAcknowledgment, result.Kind);
        Assert.Null(result.Cfdi);
        CancelacionData? acuse = result.Cancelacion;
        Assert.NotNull(acuse);
        Assert.Equal("EKU9003173C9", acuse.RfcEmisor);
        Assert.Equal(new DateTime(2026, 10, 20, 10, 46, 21), acuse.Fecha);
        Assert.Equal(2, acuse.Folios.Count);
    }

    [Fact]
    public void Read_AcusePax_ReadsFirstFolio()
    {
        XmlReadResult result = ReadWithReader("acuse-pax.xml");
        CancelacionData? acuse = result.Cancelacion;
        Assert.NotNull(acuse);
        FolioData folio = acuse.Folios[0];
        Assert.Equal("6F3A2B1C-4D5E-4F60-8A7B-9C0D1E2F3A4B", folio.UUID);
        Assert.Equal("201", folio.UUIDEstatus);
        Assert.Equal("201 - Comprobante Cancelado sin Aceptación", folio.UUIDdescripcion);
        Assert.Equal(new DateTime(2026, 10, 20, 10, 46, 21), folio.UUIDfecha);
    }

    [Fact]
    public void Read_AcusePax_KeepsSecondFolioAsWritten()
    {
        XmlReadResult result = ReadWithReader("acuse-pax.xml");
        CancelacionData? acuse = result.Cancelacion;
        Assert.NotNull(acuse);
        FolioData folio = acuse.Folios[1];
        Assert.Equal("24bc32f1-44ea-4f58-9787-b02fce07ea73", folio.UUID);
        Assert.Equal("202", folio.UUIDEstatus);
        Assert.Equal(new DateTime(2026, 10, 20, 10, 46, 23), folio.UUIDfecha);
    }

    [Fact]
    public void Read_AcuseWithXmlSignature_IgnoresSignatureAndUnknownElements()
    {
        XmlReadResult result = ReadWithReader("acuse-firma-xml.xml");
        Assert.Equal(XmlReadStatus.Read, result.Status);
        Assert.Empty(result.StructuralErrors);
        CancelacionData? acuse = result.Cancelacion;
        Assert.NotNull(acuse);
        FolioData folio = Assert.Single(acuse.Folios);
        Assert.Equal("9B2E4C1A-5D6F-4A7B-8C9D-0E1F2A3B4C5D", folio.UUID);
        Assert.Equal("201", folio.UUIDEstatus);
        Assert.Null(folio.UUIDdescripcion);
        Assert.Null(folio.UUIDfecha);
    }

    [Fact]
    public void Read_AcuseWithoutFolios_ReturnsInvalid()
    {
        XmlReadResult result = ReadWithReader("acuse-sin-folios.xml");
        Assert.Equal(XmlReadStatus.Invalid, result.Status);
        Assert.Equal(DocumentKind.CancellationAcknowledgment, result.Kind);
        Assert.Null(result.Cancelacion);
        StructuralError error = Assert.Single(result.StructuralErrors);
        Assert.Equal(2, error.LineNumber);
        Assert.Equal("El acuse no tiene elementos Folios.", error.Message);
    }

    [Fact]
    public void Read_AcuseWithInvalidValues_ReturnsBothErrors()
    {
        XmlReadResult result = ReadWithReader("acuse-valores-invalidos.xml");
        Assert.Equal(XmlReadStatus.Invalid, result.Status);
        Assert.Null(result.Cancelacion);
        IReadOnlyList<StructuralError> errors = result.StructuralErrors;
        Assert.Equal(2, errors.Count);
        Assert.Equal(2, errors[0].LineNumber);
        Assert.Equal(4, errors[1].LineNumber);
    }

    [Fact]
    public void Read_SoapResponse_ReturnsUnrecognized()
    {
        XmlReadResult result = ReadWithReader("respuesta-soap.xml");
        Assert.Equal(XmlReadStatus.Unrecognized, result.Status);
        Assert.Null(result.Kind);
        Assert.Null(result.Cancelacion);
    }

    [Fact]
    public void Verify_AcusePax_ReturnsNoErrors()
    {
        byte[] content = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fixtures", "Cancelacion", "acuse-pax.xml"));
        SecureXmlLoader loader = new SecureXmlLoader();
        SecureXmlLoadResult loaded = loader.Load(content);
        XDocument? document = loaded.Document;
        Assert.NotNull(document);
        IReadOnlyList<StructuralError> errors = CancelacionVerifier.Verify(document);
        Assert.Empty(errors);
    }

    private static XmlReadResult ReadWithReader(string fileName)
    {
        ServiceCollection services = new ServiceCollection();
        services.AddDanmaobCfdiCore();
        ServiceProvider provider = services.BuildServiceProvider();
        IThirdPartyXmlReader reader = provider.GetRequiredService<IThirdPartyXmlReader>();
        byte[] content = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fixtures", "Cancelacion", fileName));
        return reader.Read(content);
    }
}
