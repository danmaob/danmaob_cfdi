using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace DanmaobCfdi.Core.Reading.CartaPorte31;

internal static class CartaPorte31Extractor
{
    private static readonly XName ComplementoName = XName.Get("Complemento", "http://www.sat.gob.mx/cfd/4");
    private static readonly XNamespace Ccp31 = "http://www.sat.gob.mx/CartaPorte31";

    internal static CartaPorteData? Extract(XElement comprobante)
    {
        XElement? complemento = comprobante.Element(ComplementoName);
        if (complemento is null)
        {
            return null;
        }

        XElement? cartaPorte = complemento.Element(Ccp31 + "CartaPorte");
        if (cartaPorte is null)
        {
            return null;
        }

        List<UbicacionData> ubicaciones = new List<UbicacionData>();
        foreach (XElement ubicacion in cartaPorte.Elements(Ccp31 + "Ubicaciones").Elements(Ccp31 + "Ubicacion"))
        {
            ubicaciones.Add(ReadUbicacion(ubicacion));
        }

        return new CartaPorteData
        {
            Version = XmlAttributeReader.Required(cartaPorte, "Version"),
            IdCCP = XmlAttributeReader.Required(cartaPorte, "IdCCP"),
            TranspInternac = XmlAttributeReader.Required(cartaPorte, "TranspInternac"),
            EntradaSalidaMerc = XmlAttributeReader.Optional(cartaPorte, "EntradaSalidaMerc"),
            PaisOrigenDestino = XmlAttributeReader.Optional(cartaPorte, "PaisOrigenDestino"),
            ViaEntradaSalida = XmlAttributeReader.Optional(cartaPorte, "ViaEntradaSalida"),
            TotalDistRec = XmlAttributeReader.OptionalDecimal(cartaPorte, "TotalDistRec"),
            Ubicaciones = ubicaciones,
            Mercancias = ReadMercancias(cartaPorte.Element(Ccp31 + "Mercancias")),
        };
    }

    private static UbicacionData ReadUbicacion(XElement ubicacion)
    {
        return new UbicacionData
        {
            TipoUbicacion = XmlAttributeReader.Required(ubicacion, "TipoUbicacion"),
            IDUbicacion = XmlAttributeReader.Optional(ubicacion, "IDUbicacion"),
            RFCRemitenteDestinatario = XmlAttributeReader.Required(ubicacion, "RFCRemitenteDestinatario"),
            NombreRemitenteDestinatario = XmlAttributeReader.Optional(ubicacion, "NombreRemitenteDestinatario"),
            NumRegIdTrib = XmlAttributeReader.Optional(ubicacion, "NumRegIdTrib"),
            ResidenciaFiscal = XmlAttributeReader.Optional(ubicacion, "ResidenciaFiscal"),
            FechaHoraSalidaLlegada = XmlAttributeReader.RequiredDateTime(ubicacion, "FechaHoraSalidaLlegada"),
            DistanciaRecorrida = XmlAttributeReader.OptionalDecimal(ubicacion, "DistanciaRecorrida"),
            Domicilio = ReadDomicilio(ubicacion.Element(Ccp31 + "Domicilio")),
        };
    }

    private static DomicilioData? ReadDomicilio(XElement? domicilio)
    {
        if (domicilio is null)
        {
            return null;
        }

        return new DomicilioData
        {
            Calle = XmlAttributeReader.Optional(domicilio, "Calle"),
            NumeroExterior = XmlAttributeReader.Optional(domicilio, "NumeroExterior"),
            NumeroInterior = XmlAttributeReader.Optional(domicilio, "NumeroInterior"),
            Colonia = XmlAttributeReader.Optional(domicilio, "Colonia"),
            Localidad = XmlAttributeReader.Optional(domicilio, "Localidad"),
            Referencia = XmlAttributeReader.Optional(domicilio, "Referencia"),
            Municipio = XmlAttributeReader.Optional(domicilio, "Municipio"),
            Estado = XmlAttributeReader.Required(domicilio, "Estado"),
            Pais = XmlAttributeReader.Required(domicilio, "Pais"),
            CodigoPostal = XmlAttributeReader.Required(domicilio, "CodigoPostal"),
        };
    }

    private static MercanciasData ReadMercancias(XElement? mercancias)
    {
        if (mercancias is null)
        {
            return new MercanciasData();
        }

        List<MercanciaData> lista = new List<MercanciaData>();
        foreach (XElement mercancia in mercancias.Elements(Ccp31 + "Mercancia"))
        {
            lista.Add(ReadMercancia(mercancia));
        }

        return new MercanciasData
        {
            PesoBrutoTotal = XmlAttributeReader.RequiredDecimal(mercancias, "PesoBrutoTotal"),
            UnidadPeso = XmlAttributeReader.Required(mercancias, "UnidadPeso"),
            PesoNetoTotal = XmlAttributeReader.OptionalDecimal(mercancias, "PesoNetoTotal"),
            NumTotalMercancias = int.Parse(XmlAttributeReader.Required(mercancias, "NumTotalMercancias"), CultureInfo.InvariantCulture),
            Mercancias = lista,
        };
    }

    private static MercanciaData ReadMercancia(XElement mercancia)
    {
        return new MercanciaData
        {
            BienesTransp = XmlAttributeReader.Required(mercancia, "BienesTransp"),
            Descripcion = XmlAttributeReader.Required(mercancia, "Descripcion"),
            Cantidad = XmlAttributeReader.RequiredDecimal(mercancia, "Cantidad"),
            ClaveUnidad = XmlAttributeReader.Required(mercancia, "ClaveUnidad"),
            Unidad = XmlAttributeReader.Optional(mercancia, "Unidad"),
            MaterialPeligroso = XmlAttributeReader.Optional(mercancia, "MaterialPeligroso"),
            CveMaterialPeligroso = XmlAttributeReader.Optional(mercancia, "CveMaterialPeligroso"),
            Embalaje = XmlAttributeReader.Optional(mercancia, "Embalaje"),
            PesoEnKg = XmlAttributeReader.RequiredDecimal(mercancia, "PesoEnKg"),
            ValorMercancia = XmlAttributeReader.OptionalDecimal(mercancia, "ValorMercancia"),
            Moneda = XmlAttributeReader.Optional(mercancia, "Moneda"),
            FraccionArancelaria = XmlAttributeReader.Optional(mercancia, "FraccionArancelaria"),
        };
    }
}
