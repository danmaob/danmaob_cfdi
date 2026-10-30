using System;
using System.IO;
using System.Xml.Linq;
using DanmaobCfdi.Core.Reading;
using DanmaobCfdi.Core.Reading.Cfdi40;
using Xunit;

namespace DanmaobCfdi.Tests.Core.Reading.Cfdi40;

public sealed class Cfdi40ExtractorTests
{
    [Fact]
    public void Extract_IngresoFlete_ReadsComprobanteAttributes()
    {
        ComprobanteData data = ExtractFixture("ingreso-flete.xml");

        Assert.Equal("4.0", data.Version);
        Assert.Equal("F", data.Serie);
        Assert.Equal("1024", data.Folio);
        Assert.Equal(new DateTime(2026, 10, 16, 11, 30, 0), data.Fecha);
        Assert.Equal(DateTimeKind.Unspecified, data.Fecha.Kind);
        Assert.Equal("99", data.FormaPago);
        Assert.Equal("CREDITO A 30 DIAS", data.CondicionesDePago);
        Assert.Equal(2500.00m, data.SubTotal);
        Assert.Equal(100.00m, data.Descuento);
        Assert.Equal("MXN", data.Moneda);
        Assert.Null(data.TipoCambio);
        Assert.Equal(2688.00m, data.Total);
        Assert.Equal("I", data.TipoDeComprobante);
        Assert.Equal("01", data.Exportacion);
        Assert.Equal("PPD", data.MetodoPago);
        Assert.Equal("42501", data.LugarExpedicion);
        Assert.Empty(data.CfdiRelacionados);
    }

    [Fact]
    public void Extract_IngresoFlete_ReadsEmisorAndReceptor()
    {
        ComprobanteData data = ExtractFixture("ingreso-flete.xml");

        Assert.Equal("EKU9003173C9", data.Emisor.Rfc);
        Assert.Equal("ESCUELA KEMPER URGATE", data.Emisor.Nombre);
        Assert.Equal("601", data.Emisor.RegimenFiscal);
        Assert.Null(data.Emisor.FacAtrAdquirente);
        Assert.Equal("WERX631016S30", data.Receptor.Rfc);
        Assert.Equal("06300", data.Receptor.DomicilioFiscalReceptor);
        Assert.Equal("625", data.Receptor.RegimenFiscalReceptor);
        Assert.Equal("G03", data.Receptor.UsoCFDI);
        Assert.Null(data.Receptor.ResidenciaFiscal);
    }

    [Fact]
    public void Extract_IngresoFlete_ReadsConceptosWithTaxes()
    {
        ComprobanteData data = ExtractFixture("ingreso-flete.xml");

        Assert.Equal(2, data.Conceptos.Count);
        ConceptoData flete = data.Conceptos[0];

        Assert.Equal("78101802", flete.ClaveProdServ);
        Assert.Equal("VIAJE-001", flete.NoIdentificacion);
        Assert.Equal(1m, flete.Cantidad);
        Assert.Equal("E48", flete.ClaveUnidad);
        Assert.Equal(2000.00m, flete.Importe);
        Assert.Equal(100.00m, flete.Descuento);
        Assert.Equal("02", flete.ObjetoImp);
        TrasladoData traslado = Assert.Single(flete.Traslados);

        Assert.Equal(1900.00m, traslado.Base);
        Assert.Equal("002", traslado.Impuesto);
        Assert.Equal("Tasa", traslado.TipoFactor);
        Assert.Equal(0.160000m, traslado.TasaOCuota);
        Assert.Equal(304.00m, traslado.Importe);
        RetencionData retencion = Assert.Single(flete.Retenciones);

        Assert.Equal(1900.00m, retencion.Base);
        Assert.Equal(0.040000m, retencion.TasaOCuota);
        Assert.Equal(76.00m, retencion.Importe);
        ConceptoData maniobras = data.Conceptos[1];

        Assert.Null(maniobras.NoIdentificacion);
        Assert.Equal(2m, maniobras.Cantidad);
        Assert.Equal(250.00m, maniobras.ValorUnitario);
        Assert.Null(maniobras.Descuento);
    }

    [Fact]
    public void Extract_IngresoFlete_ReadsDocumentTaxes()
    {
        ComprobanteData data = ExtractFixture("ingreso-flete.xml");

        ImpuestosData? impuestos = data.Impuestos;
        Assert.NotNull(impuestos);
        Assert.Equal(96.00m, impuestos.TotalImpuestosRetenidos);
        Assert.Equal(384.00m, impuestos.TotalImpuestosTrasladados);
        TrasladoData traslado = Assert.Single(impuestos.Traslados);

        Assert.Equal(2400.00m, traslado.Base);
        Assert.Equal(384.00m, traslado.Importe);
        RetencionData retencion = Assert.Single(impuestos.Retenciones);

        Assert.Null(retencion.Base);
        Assert.Null(retencion.TipoFactor);
        Assert.Equal("002", retencion.Impuesto);
        Assert.Equal(96.00m, retencion.Importe);
    }

    [Fact]
    public void Extract_EgresoRelacionado_ReadsCfdiRelacionados()
    {
        ComprobanteData data = ExtractFixture("egreso-relacionado.xml");

        Assert.Equal("E", data.TipoDeComprobante);
        Assert.Equal(2, data.CfdiRelacionados.Count);
        CfdiRelacionadosData notaDeCredito = data.CfdiRelacionados[0];

        Assert.Equal("01", notaDeCredito.TipoRelacion);
        Assert.Equal(2, notaDeCredito.Uuids.Count);
        Assert.Equal("5FB2822E-396D-4725-8521-CDC4BDD20CCF", notaDeCredito.Uuids[0]);
        Assert.Equal("1A2B3C4D-5E6F-4A7B-8C9D-0E1F2A3B4C5D", notaDeCredito.Uuids[1]);
        CfdiRelacionadosData anticipo = data.CfdiRelacionados[1];

        Assert.Equal("07", anticipo.TipoRelacion);
        string uuid = Assert.Single(anticipo.Uuids);
        Assert.Equal("9E8D7C6B-5A49-4382-A716-05F4E3D2C1B0", uuid);
    }

    [Fact]
    public void Extract_ReferenceCfdi_LeavesAbsentOptionalValuesNull()
    {
        ComprobanteData data = ExtractFixture("ingreso-basico.xml");

        Assert.Null(data.Descuento);
        Assert.Null(data.CondicionesDePago);
        Assert.Equal("PUE", data.MetodoPago);
        Assert.Empty(data.CfdiRelacionados);
        ConceptoData concepto = Assert.Single(data.Conceptos);

        Assert.Null(concepto.NoIdentificacion);
        Assert.Empty(concepto.Retenciones);
        ImpuestosData? impuestos = data.Impuestos;
        Assert.NotNull(impuestos);
        Assert.Null(impuestos.TotalImpuestosRetenidos);
        Assert.Empty(impuestos.Retenciones);
    }

    private static ComprobanteData ExtractFixture(string fileName)
    {
        byte[] content = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fixtures", "Cfdi40", fileName));
        SecureXmlLoader loader = new SecureXmlLoader();
        SecureXmlLoadResult result = loader.Load(content);
        XDocument? document = result.Document;
        Assert.NotNull(document);
        Cfdi40Extractor extractor = new Cfdi40Extractor();
        return extractor.Extract(document);
    }
}
