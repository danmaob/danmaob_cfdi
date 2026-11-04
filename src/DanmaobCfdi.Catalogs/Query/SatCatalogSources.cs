using System;
using System.Linq;
using DanmaobCfdi.Catalogs.CartaPorte31;
using DanmaobCfdi.Catalogs.Cfdi40;
using DanmaobCfdi.Catalogs.ComercioExterior;
using DanmaobCfdi.Catalogs.Persistence;
using DanmaobCfdi.Core.Catalogs;
namespace DanmaobCfdi.Catalogs.Query;

internal static class SatCatalogSources
{
    internal static IQueryable<SatCatalogEntity> For(SatDbContext context, SatCatalog catalog)
    {
        switch (catalog)
        {
            case SatCatalog.Aduana:
                return context.Set<Aduana>();
            case SatCatalog.ClaveProdServ:
                return context.Set<ClaveProdServ>();
            case SatCatalog.ClaveUnidad:
                return context.Set<ClaveUnidad>();
            case SatCatalog.CodigoPostal:
                return context.Set<CodigoPostal>();
            case SatCatalog.Colonia:
                return context.Set<Colonia>();
            case SatCatalog.Estado:
                return context.Set<Estado>();
            case SatCatalog.Exportacion:
                return context.Set<Exportacion>();
            case SatCatalog.FormaPago:
                return context.Set<FormaPago>();
            case SatCatalog.Impuesto:
                return context.Set<Impuesto>();
            case SatCatalog.Localidad:
                return context.Set<Localidad>();
            case SatCatalog.Meses:
                return context.Set<Meses>();
            case SatCatalog.MetodoPago:
                return context.Set<MetodoPago>();
            case SatCatalog.Moneda:
                return context.Set<Moneda>();
            case SatCatalog.MotivoCancelacion:
                return context.Set<MotivoCancelacion>();
            case SatCatalog.Municipio:
                return context.Set<Municipio>();
            case SatCatalog.ObjetoImp:
                return context.Set<ObjetoImp>();
            case SatCatalog.Pais:
                return context.Set<Pais>();
            case SatCatalog.PatenteAduanal:
                return context.Set<PatenteAduanal>();
            case SatCatalog.Periodicidad:
                return context.Set<Periodicidad>();
            case SatCatalog.RegimenFiscal:
                return context.Set<RegimenFiscal>();
            case SatCatalog.TipoCadenaPago:
                return context.Set<TipoCadenaPago>();
            case SatCatalog.TipoDeComprobante:
                return context.Set<TipoDeComprobante>();
            case SatCatalog.TipoFactor:
                return context.Set<TipoFactor>();
            case SatCatalog.TipoRelacion:
                return context.Set<TipoRelacion>();
            case SatCatalog.UsoCFDI:
                return context.Set<UsoCFDI>();
            case SatCatalog.ClaveProdServCP:
                return context.Set<ClaveProdServCP>();
            case SatCatalog.ClaveTipoCarga:
                return context.Set<ClaveTipoCarga>();
            case SatCatalog.ClaveUnidadPeso:
                return context.Set<ClaveUnidadPeso>();
            case SatCatalog.CodigoTransporteAereo:
                return context.Set<CodigoTransporteAereo>();
            case SatCatalog.CondicionesEspeciales:
                return context.Set<CondicionesEspeciales>();
            case SatCatalog.ConfigAutotransporte:
                return context.Set<ConfigAutotransporte>();
            case SatCatalog.ConfigMaritima:
                return context.Set<ConfigMaritima>();
            case SatCatalog.Contenedor:
                return context.Set<Contenedor>();
            case SatCatalog.ContenedorMaritimo:
                return context.Set<ContenedorMaritimo>();
            case SatCatalog.CveTransporte:
                return context.Set<CveTransporte>();
            case SatCatalog.DerechosDePaso:
                return context.Set<DerechosDePaso>();
            case SatCatalog.DocumentoAduanero:
                return context.Set<DocumentoAduanero>();
            case SatCatalog.Estaciones:
                return context.Set<Estaciones>();
            case SatCatalog.FiguraTransporte:
                return context.Set<FiguraTransporte>();
            case SatCatalog.FormaFarmaceutica:
                return context.Set<FormaFarmaceutica>();
            case SatCatalog.NumAutorizacionNaviero:
                return context.Set<NumAutorizacionNaviero>();
            case SatCatalog.ParteTransporte:
                return context.Set<ParteTransporte>();
            case SatCatalog.RegimenAduanero:
                return context.Set<RegimenAduanero>();
            case SatCatalog.RegistroISTMO:
                return context.Set<RegistroISTMO>();
            case SatCatalog.SectorCOFEPRIS:
                return context.Set<SectorCOFEPRIS>();
            case SatCatalog.SubTipoRem:
                return context.Set<SubTipoRem>();
            case SatCatalog.TipoCarro:
                return context.Set<TipoCarro>();
            case SatCatalog.TipoDeServicio:
                return context.Set<TipoDeServicio>();
            case SatCatalog.TipoDeTrafico:
                return context.Set<TipoDeTrafico>();
            case SatCatalog.TipoEmbalaje:
                return context.Set<TipoEmbalaje>();
            case SatCatalog.TipoEstacion:
                return context.Set<TipoEstacion>();
            case SatCatalog.TipoMateria:
                return context.Set<TipoMateria>();
            case SatCatalog.TipoPermiso:
                return context.Set<TipoPermiso>();
            case SatCatalog.FraccionArancelaria:
                return context.Set<FraccionArancelaria>();
            default:
                throw new ArgumentOutOfRangeException(nameof(catalog));
        }
    }

    internal static string? ParentColumn(SatCatalog catalog)
    {
        switch (catalog)
        {
            case SatCatalog.Colonia:
                return "CodigoPostal";
            case SatCatalog.Estado:
                return "Pais";
            case SatCatalog.Municipio:
                return "Estado";
            case SatCatalog.Localidad:
                return "Estado";
            default:
                return null;
        }
    }
}
