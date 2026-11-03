using System;
using System.IO;
using System.Xml.Linq;
using DanmaobCfdi.Core.Reading;
using DanmaobCfdi.Core.Reading.CartaPorte31;
using Xunit;

namespace DanmaobCfdi.Tests.Core.Reading.CartaPorte31;

public sealed class CartaPorte31TransportTests
{
    [Fact]
    public void Extract_IngresoCartaPorte_ReadsAutotransporte()
    {
        CartaPorteData? cartaPorte = CartaPorte31Extractor.Extract(LoadRoot("Cfdi40", "ingreso-cartaporte.xml"));
        Assert.NotNull(cartaPorte);
        AutotransporteData? autotransporte = cartaPorte.Mercancias.Autotransporte;
        Assert.NotNull(autotransporte);
        Assert.Equal("TPAF01", autotransporte.PermSCT);
        Assert.Equal("PERMISO-SCT-0001", autotransporte.NumPermisoSCT);
        IdentificacionVehicularData vehiculo = autotransporte.IdentificacionVehicular;
        Assert.Equal("T3S2", vehiculo.ConfigVehicular);
        Assert.Equal(41.00m, vehiculo.PesoBrutoVehicular);
        Assert.Equal("ABC1234", vehiculo.PlacaVM);
        Assert.Equal(2022, vehiculo.AnioModeloVM);
    }

    [Fact]
    public void Extract_IngresoCartaPorte_ReadsSeguros()
    {
        CartaPorteData? cartaPorte = CartaPorte31Extractor.Extract(LoadRoot("Cfdi40", "ingreso-cartaporte.xml"));
        Assert.NotNull(cartaPorte);
        AutotransporteData? autotransporte = cartaPorte.Mercancias.Autotransporte;
        Assert.NotNull(autotransporte);
        SegurosData seguros = autotransporte.Seguros;
        Assert.Equal("SEGUROS DE PRUEBA", seguros.AseguraRespCivil);
        Assert.Equal("POL-123456", seguros.PolizaRespCivil);
        Assert.Null(seguros.AseguraMedAmbiente);
        Assert.Null(seguros.PolizaMedAmbiente);
        Assert.Null(seguros.AseguraCarga);
        Assert.Null(seguros.PolizaCarga);
        Assert.Null(seguros.PrimaSeguro);
    }

    [Fact]
    public void Extract_IngresoCartaPorte_ReadsRemolque()
    {
        CartaPorteData? cartaPorte = CartaPorte31Extractor.Extract(LoadRoot("Cfdi40", "ingreso-cartaporte.xml"));
        Assert.NotNull(cartaPorte);
        AutotransporteData? autotransporte = cartaPorte.Mercancias.Autotransporte;
        Assert.NotNull(autotransporte);
        RemolqueData remolque = Assert.Single(autotransporte.Remolques);
        Assert.Equal("CTR007", remolque.SubTipoRem);
        Assert.Equal("REM1234", remolque.Placa);
    }

    [Fact]
    public void Extract_IngresoCartaPorte_ReadsOperador()
    {
        CartaPorteData? cartaPorte = CartaPorte31Extractor.Extract(LoadRoot("Cfdi40", "ingreso-cartaporte.xml"));
        Assert.NotNull(cartaPorte);
        TiposFiguraData operador = Assert.Single(cartaPorte.FiguraTransporte);
        Assert.Equal("01", operador.TipoFigura);
        Assert.Equal("XAMA620210DQ5", operador.RFCFigura);
        Assert.Equal("LIC1234567", operador.NumLicencia);
        Assert.Equal("OPERADOR DE PRUEBA", operador.NombreFigura);
        Assert.Null(operador.NumRegIdTribFigura);
        Assert.Null(operador.ResidenciaFiscalFigura);
        Assert.Empty(operador.PartesTransporte);
        Assert.Null(operador.Domicilio);
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
}
