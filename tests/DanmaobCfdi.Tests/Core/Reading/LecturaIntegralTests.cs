using System;
using System.IO;
using DanmaobCfdi.Core;
using DanmaobCfdi.Core.Reading;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DanmaobCfdi.Tests.Core.Reading;

public sealed class LecturaIntegralTests
{
    private static readonly IThirdPartyXmlReader Reader = CreateReader();

    [Theory]
    [InlineData("Cfdi40", "ingreso-basico.xml", XmlReadStatus.Read, DocumentKind.Cfdi40)]
    [InlineData("Cfdi40", "ingreso-flete.xml", XmlReadStatus.Read, DocumentKind.Cfdi40)]
    [InlineData("Cfdi40", "ingreso-timbrado.xml", XmlReadStatus.Read, DocumentKind.Cfdi40)]
    [InlineData("Cfdi40", "egreso-relacionado.xml", XmlReadStatus.Read, DocumentKind.Cfdi40)]
    [InlineData("Cfdi40", "pago-parcialidad.xml", XmlReadStatus.Read, DocumentKind.Cfdi40)]
    [InlineData("Cfdi40", "ingreso-cartaporte.xml", XmlReadStatus.Read, DocumentKind.Cfdi40)]
    [InlineData("Cfdi40", "con-extensiones-desconocidas.xml", XmlReadStatus.Read, DocumentKind.Cfdi40)]
    [InlineData("Cancelacion", "acuse-pax.xml", XmlReadStatus.Read, DocumentKind.CancellationAcknowledgment)]
    [InlineData("Cancelacion", "acuse-firma-xml.xml", XmlReadStatus.Read, DocumentKind.CancellationAcknowledgment)]
    [InlineData("Cancelacion", "acuse-sin-folios.xml", XmlReadStatus.Invalid, DocumentKind.CancellationAcknowledgment)]
    [InlineData("Cancelacion", "acuse-valores-invalidos.xml", XmlReadStatus.Invalid, DocumentKind.CancellationAcknowledgment)]
    [InlineData("Cancelacion", "respuesta-soap.xml", XmlReadStatus.Unrecognized, null)]
    [InlineData("Deteccion", "acuse-cancelacion.xml", XmlReadStatus.Read, DocumentKind.CancellationAcknowledgment)]
    [InlineData("Deteccion", "cancelacion-otro-namespace.xml", XmlReadStatus.Unrecognized, null)]
    [InlineData("Deteccion", "cfdi40-cartaporte31.xml", XmlReadStatus.Invalid, DocumentKind.Cfdi40)]
    [InlineData("Deteccion", "no-reconocido.xml", XmlReadStatus.Unrecognized, null)]
    [InlineData("Invalidos", "atributo-fuera-del-xsd.xml", XmlReadStatus.Invalid, DocumentKind.Cfdi40)]
    [InlineData("Invalidos", "tfd-atributo-fuera-del-xsd.xml", XmlReadStatus.Invalid, DocumentKind.Cfdi40)]
    [InlineData("Invalidos", "varios-errores.xml", XmlReadStatus.Invalid, DocumentKind.Cfdi40)]
    [InlineData("Invalidos", "con-dtd.xml", XmlReadStatus.Rejected, null)]
    [InlineData("Invalidos", "entidad-externa.xml", XmlReadStatus.Rejected, null)]
    [InlineData("Invalidos", "mal-formado.xml", XmlReadStatus.Rejected, null)]
    [InlineData("Invalidos", "declara-iso-8859-1.xml", XmlReadStatus.Rejected, null)]
    [InlineData("Invalidos", "utf8-invalido.xml", XmlReadStatus.Rejected, null)]
    [InlineData("Invalidos", "utf16.xml", XmlReadStatus.Rejected, null)]
    [InlineData("NoSoportados", "cfdi32.xml", XmlReadStatus.Unsupported, null)]
    [InlineData("NoSoportados", "cfdi33.xml", XmlReadStatus.Unsupported, null)]
    [InlineData("NoSoportados", "cartaporte30.xml", XmlReadStatus.Unsupported, null)]
    [InlineData("NoSoportados", "pagos10.xml", XmlReadStatus.Unsupported, null)]
    [InlineData("NoSoportados", "tfd10.xml", XmlReadStatus.Unsupported, null)]
    [InlineData("NoSoportados", "retenciones20.xml", XmlReadStatus.Unsupported, null)]
    public void Read_ReferenceFixture_ReturnsExpectedStatusAndKind(string folder, string fileName, XmlReadStatus expectedStatus, DocumentKind? expectedKind)
    {
        byte[] content = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fixtures", folder, fileName));
        XmlReadResult result = Reader.Read(content);
        Assert.Equal(expectedStatus, result.Status);
        Assert.Equal(expectedKind, result.Kind);
        Assert.Equal(expectedStatus == XmlReadStatus.Invalid, result.StructuralErrors.Count > 0);
        Assert.Equal(expectedStatus == XmlReadStatus.Read && expectedKind == DocumentKind.Cfdi40, result.Cfdi is not null);
        Assert.Equal(expectedStatus == XmlReadStatus.Read && expectedKind == DocumentKind.CancellationAcknowledgment, result.Cancelacion is not null);
    }

    private static IThirdPartyXmlReader CreateReader()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddDanmaobCfdiCore();
        ServiceProvider provider = services.BuildServiceProvider();
        return provider.GetRequiredService<IThirdPartyXmlReader>();
    }
}
