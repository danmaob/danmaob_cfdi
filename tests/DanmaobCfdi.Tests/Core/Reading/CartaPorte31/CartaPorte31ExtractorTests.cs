using System;
using System.IO;
using System.Xml.Linq;
using DanmaobCfdi.Core;
using DanmaobCfdi.Core.Reading;
using DanmaobCfdi.Core.Reading.CartaPorte31;
using DanmaobCfdi.Core.Reading.Cfdi40;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DanmaobCfdi.Tests.Core.Reading.CartaPorte31;

public sealed class CartaPorte31ExtractorTests
{
    [Fact]
    public void Extract_IngresoCartaPorte_ReadsCartaPorteAttributes()
    {
        CartaPorteData? cartaPorte = CartaPorte31Extractor.Extract(LoadRoot("Cfdi40", "ingreso-cartaporte.xml"));

        Assert.NotNull(cartaPorte);
        Assert.Equal("3.1", cartaPorte.Version);
        Assert.Equal("CCC5A1B2-3C4D-4E5F-8A9B-0C1D2E3F4A5B", cartaPorte.IdCCP);
        Assert.Equal("No", cartaPorte.TranspInternac);
        Assert.Null(cartaPorte.EntradaSalidaMerc);
        Assert.Null(cartaPorte.PaisOrigenDestino);
        Assert.Null(cartaPorte.ViaEntradaSalida);
        Assert.Equal(780.5m, cartaPorte.TotalDistRec);
        Assert.Equal(2, cartaPorte.Ubicaciones.Count);
    }
    [Fact]
    public void Extract_IngresoCartaPorte_ReadsOrigenWithDomicilio()
    {
        CartaPorteData? cartaPorte = CartaPorte31Extractor.Extract(LoadRoot("Cfdi40", "ingreso-cartaporte.xml"));

        Assert.NotNull(cartaPorte);
        UbicacionData origen = cartaPorte.Ubicaciones[0];
        Assert.Equal("Origen", origen.TipoUbicacion);
        Assert.Equal("OR000001", origen.IDUbicacion);
        Assert.Equal("EKU9003173C9", origen.RFCRemitenteDestinatario);
        Assert.Equal("ESCUELA KEMPER URGATE", origen.NombreRemitenteDestinatario);
        Assert.Equal(new DateTime(2026, 10, 16, 8, 0, 0), origen.FechaHoraSalidaLlegada);
        Assert.Null(origen.DistanciaRecorrida);
        DomicilioData? domicilio = origen.Domicilio;
        Assert.NotNull(domicilio);
        Assert.Equal("Avenida Guerrero", domicilio.Calle);
        Assert.Equal("100", domicilio.NumeroExterior);
        Assert.Null(domicilio.NumeroInterior);
        Assert.Null(domicilio.Colonia);
        Assert.Equal("017", domicilio.Municipio);
        Assert.Equal("GUA", domicilio.Estado);
        Assert.Equal("MEX", domicilio.Pais);
        Assert.Equal("36500", domicilio.CodigoPostal);
    }
    [Fact]
    public void Extract_IngresoCartaPorte_ReadsDestinoWithDistance()
    {
        CartaPorteData? cartaPorte = CartaPorte31Extractor.Extract(LoadRoot("Cfdi40", "ingreso-cartaporte.xml"));

        Assert.NotNull(cartaPorte);
        UbicacionData destino = cartaPorte.Ubicaciones[1];
        Assert.Equal("Destino", destino.TipoUbicacion);
        Assert.Equal("DE000001", destino.IDUbicacion);
        Assert.Equal("WERX631016S30", destino.RFCRemitenteDestinatario);
        Assert.Null(destino.NumRegIdTrib);
        Assert.Null(destino.ResidenciaFiscal);
        Assert.Equal(new DateTime(2026, 10, 16, 20, 30, 0), destino.FechaHoraSalidaLlegada);
        Assert.Equal(780.5m, destino.DistanciaRecorrida);
        DomicilioData? domicilio = destino.Domicilio;
        Assert.NotNull(domicilio);
        Assert.Equal("039", domicilio.Municipio);
        Assert.Equal("NLE", domicilio.Estado);
        Assert.Equal("64000", domicilio.CodigoPostal);
    }
    [Fact]
    public void Extract_IngresoCartaPorte_ReadsMercanciasTotals()
    {
        CartaPorteData? cartaPorte = CartaPorte31Extractor.Extract(LoadRoot("Cfdi40", "ingreso-cartaporte.xml"));

        Assert.NotNull(cartaPorte);
        MercanciasData mercancias = cartaPorte.Mercancias;
        Assert.Equal(10500.500m, mercancias.PesoBrutoTotal);
        Assert.Equal("KGM", mercancias.UnidadPeso);
        Assert.Null(mercancias.PesoNetoTotal);
        Assert.Equal(2, mercancias.NumTotalMercancias);
        Assert.Equal(2, mercancias.Mercancias.Count);
    }
    [Fact]
    public void Extract_IngresoCartaPorte_ReadsGeneralMercancia()
    {
        CartaPorteData? cartaPorte = CartaPorte31Extractor.Extract(LoadRoot("Cfdi40", "ingreso-cartaporte.xml"));

        Assert.NotNull(cartaPorte);
        MercanciaData herrajes = cartaPorte.Mercancias.Mercancias[0];
        Assert.Equal("31162800", herrajes.BienesTransp);
        Assert.Equal("Herrajes de acero", herrajes.Descripcion);
        Assert.Equal(100m, herrajes.Cantidad);
        Assert.Equal("H87", herrajes.ClaveUnidad);
        Assert.Equal("Pieza", herrajes.Unidad);
        Assert.Null(herrajes.MaterialPeligroso);
        Assert.Null(herrajes.CveMaterialPeligroso);
        Assert.Null(herrajes.Embalaje);
        Assert.Equal(7300.500m, herrajes.PesoEnKg);
        Assert.Equal(150000.00m, herrajes.ValorMercancia);
        Assert.Equal("MXN", herrajes.Moneda);
        Assert.Null(herrajes.FraccionArancelaria);
    }
    [Fact]
    public void Extract_IngresoCartaPorte_ReadsMaterialPeligroso()
    {
        CartaPorteData? cartaPorte = CartaPorte31Extractor.Extract(LoadRoot("Cfdi40", "ingreso-cartaporte.xml"));

        Assert.NotNull(cartaPorte);
        MercanciaData gasolina = cartaPorte.Mercancias.Mercancias[1];
        Assert.Equal("15101514", gasolina.BienesTransp);
        Assert.Equal(20m, gasolina.Cantidad);
        Assert.Equal("Tambor", gasolina.Unidad);
        Assert.Equal("Sí", gasolina.MaterialPeligroso);
        Assert.Equal("1203", gasolina.CveMaterialPeligroso);
        Assert.Equal("1A1", gasolina.Embalaje);
        Assert.Equal(3200.000m, gasolina.PesoEnKg);
    }
    [Fact]
    public void Extract_CfdiWithoutCartaPorte_ReturnsNull()
    {
        CartaPorteData? cartaPorte = CartaPorte31Extractor.Extract(LoadRoot("Cfdi40", "ingreso-timbrado.xml"));

        Assert.Null(cartaPorte);
    }
    [Fact]
    public void Read_IngresoCartaPorte_ReturnsCartaPorteInsideCfdi()
    {
        XmlReadResult result = ReadWithReader("Cfdi40", "ingreso-cartaporte.xml");

        Assert.Equal(XmlReadStatus.Read, result.Status);
        ComprobanteData? cfdi = result.Cfdi;
        Assert.NotNull(cfdi);
        Assert.Equal("I", cfdi.TipoDeComprobante);
        Assert.NotNull(cfdi.CartaPorte);
        Assert.NotNull(cfdi.TimbreFiscalDigital);
        Assert.Empty(result.UnreadComplements);
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
