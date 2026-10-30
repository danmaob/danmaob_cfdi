using System;
using System.IO;
using System.Xml.Linq;
using DanmaobCfdi.Core;
using DanmaobCfdi.Core.Reading;
using DanmaobCfdi.Core.Reading.Cfdi40;
using DanmaobCfdi.Core.Reading.Pagos20;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DanmaobCfdi.Tests.Core.Reading.Pagos20;

public sealed class Pagos20ExtractorTests
{
    [Fact]
    public void Extract_PagoParcialidad_ReadsTotalesAndPago()
    {
        PagosData? pagos = Pagos20Extractor.Extract(LoadRoot("Cfdi40", "pago-parcialidad.xml"));
        Assert.NotNull(pagos);
        Assert.Equal("2.0", pagos.Version);
        Assert.Equal(2264.00m, pagos.Totales.MontoTotalPagos);
        Assert.Equal(48.00m, pagos.Totales.TotalRetencionesIVA);
        Assert.Equal(1200.00m, pagos.Totales.TotalTrasladosBaseIVA16);
        Assert.Equal(192.00m, pagos.Totales.TotalTrasladosImpuestoIVA16);
        Assert.Null(pagos.Totales.TotalRetencionesISR);
        PagoData pago = Assert.Single(pagos.Pagos);
        Assert.Equal(new DateTime(2026, 10, 20, 9, 30, 0), pago.FechaPago);
        Assert.Equal("03", pago.FormaDePagoP);
        Assert.Equal("MXN", pago.MonedaP);
        Assert.Equal(1m, pago.TipoCambioP);
        Assert.Equal(2264.00m, pago.Monto);
        Assert.Equal("SPEI-0001", pago.NumOperacion);
    }

    [Fact]
    public void Extract_PagoParcialidad_ReadsDoctosRelacionadosWithBalances()
    {
        PagosData? pagos = Pagos20Extractor.Extract(LoadRoot("Cfdi40", "pago-parcialidad.xml"));
        Assert.NotNull(pagos);
        PagoData pago = Assert.Single(pagos.Pagos);
        Assert.Equal(2, pago.DoctosRelacionados.Count);
        DoctoRelacionadoData flete = pago.DoctosRelacionados[0];
        Assert.Equal("E3B5F1A2-7C4D-4E8F-9A1B-2C3D4E5F6A7B", flete.IdDocumento);
        Assert.Equal("F", flete.Serie);
        Assert.Equal("1025", flete.Folio);
        Assert.Equal("MXN", flete.MonedaDR);
        Assert.Equal(1, flete.NumParcialidad);
        Assert.Equal(2688.00m, flete.ImpSaldoAnt);
        Assert.Equal(1344.00m, flete.ImpPagado);
        Assert.Equal(1344.00m, flete.ImpSaldoInsoluto);
        Assert.Equal("02", flete.ObjetoImpDR);
    }

    [Fact]
    public void Extract_PagoParcialidad_KeepsTenDecimalEquivalencia()
    {
        PagosData? pagos = Pagos20Extractor.Extract(LoadRoot("Cfdi40", "pago-parcialidad.xml"));
        Assert.NotNull(pagos);
        DoctoRelacionadoData dolares = Assert.Single(pagos.Pagos).DoctosRelacionados[1];
        Assert.Equal("USD", dolares.MonedaDR);
        Assert.Equal(0.0543478261m, dolares.EquivalenciaDR);
        Assert.Equal(0.00m, dolares.ImpSaldoInsoluto);
    }

    [Fact]
    public void Extract_CfdiWithoutPagos_ReturnsNull()
    {
        PagosData? pagos = Pagos20Extractor.Extract(LoadRoot("Cfdi40", "ingreso-timbrado.xml"));
        Assert.Null(pagos);
    }

    [Fact]
    public void Read_PagoParcialidad_ReturnsPagosInsideCfdi()
    {
        XmlReadResult result = ReadWithReader("Cfdi40", "pago-parcialidad.xml");
        Assert.Equal(XmlReadStatus.Read, result.Status);
        ComprobanteData? cfdi = result.Cfdi;
        Assert.NotNull(cfdi);
        Assert.Equal("P", cfdi.TipoDeComprobante);
        Assert.NotNull(cfdi.Pagos);
        Assert.NotNull(cfdi.TimbreFiscalDigital);
        Assert.Empty(result.UnreadComplements);
    }

    [Fact]
    public void Read_Pagos10_ReturnsUnsupported()
    {
        XmlReadResult result = ReadWithReader("NoSoportados", "pagos10.xml");
        Assert.Equal(XmlReadStatus.Unsupported, result.Status);
        Assert.Equal("Pagos 1.0", result.DetectedVersion);
        Assert.Null(result.Cfdi);
    }

    private static XElement LoadRoot(string folder, string fileName)
    {
        byte[] content = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fixtures", folder, fileName));
        SecureXmlLoader loader = new SecureXmlLoader();
        SecureXmlLoadResult result = loader.Load(content);
        XDocument? document = result.Document;
        Assert.NotNull(document);
        XElement? root = document.Root;
        Assert.NotNull(root);
        return root;
    }

    private static XmlReadResult ReadWithReader(string folder, string fileName)
    {
        ServiceCollection services = new ServiceCollection();
        services.AddDanmaobCfdiCore();
        ServiceProvider provider = services.BuildServiceProvider();
        IThirdPartyXmlReader reader = provider.GetRequiredService<IThirdPartyXmlReader>();
        byte[] content = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fixtures", folder, fileName));
        return reader.Read(content);
    }
}
