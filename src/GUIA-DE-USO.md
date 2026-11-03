# DANMAOB_CFDI — Guía de uso del contrato público

Esta guía viaja con la copia del componente (`DanmaobCfdi/GUIA-DE-USO.md` dentro de cada producto) y describe exactamente la versión que la acompaña. Está escrita para el equipo que construye el módulo de facturación de un producto (DANMAOB_TMS o DANMAOB_ERP), que no necesita leer el código del componente.

- La copia nunca se edita dentro del producto. Se importa y se reemplaza con `tools/copia/reemplazar-copia.sh` del original (procedimiento `docs/procedimientos/reemplazo-de-copia.md` del original).
- La versión que tiene el producto la da `DanmaobCfdi.Core.ComponentVersion.Current` y la propiedad `Version` de `DanmaobCfdi/Directory.Build.props`.
- Lo que cambia en cada versión, y si obliga al producto a cambiar su código, está en `CHANGELOG.md` del original.

## Versiones que cubre esta guía

| Versión | Versión de corrección | Contrato público |
| --- | --- | --- |
| 1.0.0 | `CFDI_Lectura` | Lectura y validación estructural de XML de terceros |

Cada versión nueva agrega su sección y marca lo que cambió. Mientras una parte del componente no aparezca en esta tabla, no forma parte del contrato aunque su código exista.

## 1. Proyectos y registro

La copia trae tres proyectos. En la versión 1.0.0 el producto solo usa `DanmaobCfdi.Core`:

| Proyecto | En 1.0.0 |
| --- | --- |
| `DanmaobCfdi.Core` | Lectura de XML de terceros: es el contrato de esta versión |
| `DanmaobCfdi.Catalogs` | Esquema `sat` de los catálogos del SAT, todavía sin datos ni consulta. `AddDanmaobCfdiCatalogs` existe, pero no forma parte del contrato |
| `DanmaobCfdi.Pax` | Adaptador del PAC, todavía sin timbrado ni cancelación. `AddDanmaobCfdiPax` existe, pero no forma parte del contrato |

El proyecto del producto que lee XML agrega una referencia a `DanmaobCfdi/DanmaobCfdi.Core/DanmaobCfdi.Core.csproj` y registra el componente en su contenedor de servicios:

```csharp
using DanmaobCfdi.Core;

services.AddDanmaobCfdiCore();
```

`AddDanmaobCfdiCore` no recibe configuración: la lectura no usa red, base de datos, certificados ni credenciales. Los esquemas oficiales del SAT viajan dentro del componente.

## 2. Leer un XML

El único punto de entrada es la interfaz `DanmaobCfdi.Core.Reading.IThirdPartyXmlReader`, con un solo método:

```csharp
XmlReadResult Read(byte[] content);
```

- `content` son los bytes del archivo tal como lo subió el cliente. No se convierte a texto antes de leerlo.
- `Read` nunca lanza una excepción por el contenido del documento: todo problema llega en el resultado. Solo lanza `ArgumentNullException` si `content` es `null`.
- La lectura valida la estructura contra los esquemas oficiales del SAT. No verifica los sellos del emisor ni del SAT, ni consulta el estatus del comprobante ante el SAT.

```csharp
using DanmaobCfdi.Core.Reading;

public sealed class CargaDeXml
{
    private readonly IThirdPartyXmlReader _reader;

    public CargaDeXml(IThirdPartyXmlReader reader)
    {
        _reader = reader;
    }

    public void Cargar(byte[] archivo)
    {
        XmlReadResult resultado = _reader.Read(archivo);

        switch (resultado.Status)
        {
            case XmlReadStatus.Read:
                // resultado.Kind dice si es un CFDI 4.0 o un acuse de cancelación.
                break;
            case XmlReadStatus.Unsupported:
                // Versión o complemento fuera del alcance: resultado.UnsupportedReason y resultado.DetectedVersion.
                break;
            case XmlReadStatus.Unrecognized:
                // No es un CFDI ni un acuse de cancelación.
                break;
            case XmlReadStatus.Invalid:
                // No cumple la estructura: resultado.StructuralErrors.
                break;
            case XmlReadStatus.Rejected:
                // Contenido inseguro o ilegible: resultado.RejectionReason.
                break;
        }
    }
}
```

## 3. El resultado: `XmlReadResult`

| Propiedad | Tipo | Cuándo viene llena |
| --- | --- | --- |
| `Status` | `XmlReadStatus` | Siempre |
| `Kind` | `DocumentKind?` | En `Read` e `Invalid`: `Cfdi40` o `CancellationAcknowledgment`. Nula en los demás estados |
| `Cfdi` | `ComprobanteData?` | Solo en `Read` de un CFDI 4.0 |
| `Cancelacion` | `CancelacionData?` | Solo en `Read` de un acuse de cancelación |
| `UnreadComplements` | `IReadOnlyList<UnreadComplement>` | En `Read` de un CFDI que trae complementos fuera del alcance. Nunca es nula; vacía en los demás casos |
| `UnsupportedReason` | `UnsupportedReason?` | Solo en `Unsupported` |
| `DetectedVersion` | `string?` | Solo en `Unsupported`: la versión encontrada, por ejemplo `3.3` o `CartaPorte 3.0` |
| `StructuralErrors` | `IReadOnlyList<StructuralError>` | Solo en `Invalid`, con hasta 100 errores. Nunca es nula; vacía en los demás casos |
| `RejectionReason` | `RejectionReason?` | Solo en `Rejected` |

### 3.1 Los cinco estados (`XmlReadStatus`)

| Estado | Significado | Qué hace el producto |
| --- | --- | --- |
| `Read` | Documento soportado, estructura válida y modelo extraído | Usar `Cfdi` o `Cancelacion` según `Kind` |
| `Unsupported` | Documento reconocido, pero de una versión o un complemento fuera del alcance | Informar al cliente que esa versión no se acepta |
| `Unrecognized` | XML bien formado que no es un CFDI ni un acuse de cancelación | Informar que el archivo no es un comprobante |
| `Invalid` | Documento soportado que no cumple la estructura del SAT (o las reglas del acuse) | Informar que el archivo está dañado o alterado |
| `Rejected` | Archivo que no se procesó por seguridad o porque no se puede leer | Informar que el archivo no se pudo leer |

### 3.2 Versiones soportadas y motivos de `Unsupported`

Se leen: CFDI 4.0 con Timbre Fiscal Digital 1.1, Pagos 2.0 y Carta Porte 3.1, y el acuse de cancelación en XML.

| `UnsupportedReason` | Cuándo |
| --- | --- |
| `CfdiVersion` | CFDI 3.2, 3.3 o cualquier versión distinta de 4.0 |
| `ComplementVersion` | CFDI 4.0 con Carta Porte distinta de 3.1, Pagos distinto de 2.0 o Timbre Fiscal Digital distinto de 1.1 |
| `Retenciones` | Comprobante de Retenciones e información de pagos (su lectura no forma parte de esta versión) |

### 3.3 Complementos fuera del alcance (`UnreadComplements`)

Un CFDI 4.0 con un complemento que la lectura no modela (por ejemplo, Comercio Exterior) se lee igual, con estado `Read`. Cada complemento no leído aparece en `UnreadComplements` con su `NamespaceName` y su `LocalName`, para que el producto decida si lo acepta. La addenda se ignora.

Los complementos de impuestos locales y de leyendas fiscales no aparecen en `UnreadComplements`, pero la versión 1.0.0 tampoco los extrae al modelo.

### 3.4 Errores de estructura (`StructuralError`)

Cada error trae `LineNumber`, `LinePosition` y `Message`. **El mensaje puede contener valores del documento** (RFC, importes, nombres): se le puede mostrar al usuario que subió el archivo, pero el producto nunca lo escribe en un registro (ADR-05).

### 3.5 Motivos de rechazo (`RejectionReason`)

| `RejectionReason` | Cuándo |
| --- | --- |
| `TooLarge` | El archivo pesa más de 10 MB |
| `DtdOrEntity` | Declara un DTD o entidades: se rechaza por seguridad |
| `TooDeep` | Tiene más de 64 niveles de elementos anidados |
| `InvalidEncoding` | No está en UTF-8 (por ejemplo, UTF-16 o ISO-8859-1) o tiene bytes inválidos |
| `Malformed` | No es XML bien formado |

## 4. El modelo leído

Todos los tipos del modelo son inmutables: el producto los lee, nunca los construye ni los modifica. Las propiedades llevan el nombre exacto del atributo del SAT. Un atributo opcional que no viene en el documento llega en nulo; una lista sin elementos llega vacía, nunca nula.

**Las claves llegan tal como vienen en el XML, sin descripción de catálogo** (por ejemplo, `FormaPago` es `03`, no "Transferencia electrónica de fondos"). Las descripciones se agregarán cuando los catálogos tengan datos, en una versión posterior.

Las fechas llegan como `DateTime` sin zona horaria, tal como las escribe el SAT (hora del lugar de expedición). Los importes llegan como `decimal`, sin redondear.

### 4.1 CFDI 4.0 (`Kind == DocumentKind.Cfdi40`)

Espacio de nombres `DanmaobCfdi.Core.Reading.Cfdi40`.

| Tipo | Contenido |
| --- | --- |
| `ComprobanteData` | Atributos del comprobante (`Version`, `Serie`, `Folio`, `Fecha`, `FormaPago`, `CondicionesDePago`, `SubTotal`, `Descuento`, `Moneda`, `TipoCambio`, `Total`, `TipoDeComprobante`, `Exportacion`, `MetodoPago`, `LugarExpedicion`) y sus partes: `CfdiRelacionados`, `Emisor`, `Receptor`, `Conceptos`, `Impuestos`, `TimbreFiscalDigital`, `Pagos` y `CartaPorte` |
| `CfdiRelacionadosData` | `TipoRelacion` y la lista `Uuids` |
| `EmisorData` | `Rfc`, `Nombre`, `RegimenFiscal` y `FacAtrAdquirente` |
| `ReceptorData` | `Rfc`, `Nombre`, `DomicilioFiscalReceptor`, `ResidenciaFiscal`, `NumRegIdTrib`, `RegimenFiscalReceptor` y `UsoCFDI` |
| `ConceptoData` | `ClaveProdServ`, `NoIdentificacion`, `Cantidad`, `ClaveUnidad`, `Unidad`, `Descripcion`, `ValorUnitario`, `Importe`, `Descuento`, `ObjetoImp`, `Traslados` y `Retenciones` |
| `ImpuestosData` | Impuestos del comprobante: `TotalImpuestosRetenidos`, `TotalImpuestosTrasladados`, `Retenciones` y `Traslados` |
| `TrasladoData` | `Base`, `Impuesto`, `TipoFactor`, `TasaOCuota` e `Importe` |
| `RetencionData` | `Base`, `Impuesto`, `TipoFactor`, `TasaOCuota` e `Importe` |

Quedan fuera del modelo: `InformacionGlobal`, `ACuentaTerceros`, `InformacionAduanera`, `CuentaPredial`, `Parte` y `ComplementoConcepto`.

**CFDI sin timbre.** Un CFDI 4.0 sin Timbre Fiscal Digital se lee con estado `Read` y `TimbreFiscalDigital` en nulo. Aceptarlo o no en la cuenta por cobrar lo decide el producto.

### 4.2 Timbre Fiscal Digital 1.1

`ComprobanteData.TimbreFiscalDigital`, tipo `DanmaobCfdi.Core.Reading.Tfd11.TimbreFiscalDigitalData`: `Version`, `UUID`, `FechaTimbrado`, `RfcProvCertif`, `Leyenda`, `SelloCFD`, `NoCertificadoSAT` y `SelloSAT`. El `UUID` identifica el comprobante ante el SAT.

### 4.3 Pagos 2.0

`ComprobanteData.Pagos`, espacio de nombres `DanmaobCfdi.Core.Reading.Pagos20`. Nulo si el CFDI no trae el complemento.

| Tipo | Contenido |
| --- | --- |
| `PagosData` | `Version`, `Totales` y la lista `Pagos` |
| `TotalesData` | `MontoTotalPagos` y los totales de retenciones y traslados (`TotalRetencionesIVA`, `TotalRetencionesISR`, `TotalRetencionesIEPS`, `TotalTrasladosBaseIVA16`, `TotalTrasladosImpuestoIVA16`, `TotalTrasladosBaseIVA8`, `TotalTrasladosImpuestoIVA8`, `TotalTrasladosBaseIVA0`, `TotalTrasladosImpuestoIVA0` y `TotalTrasladosBaseIVAExento`) |
| `PagoData` | `FechaPago`, `FormaDePagoP`, `MonedaP`, `TipoCambioP`, `Monto`, `NumOperacion` y la lista `DoctosRelacionados` |
| `DoctoRelacionadoData` | `IdDocumento` (el UUID de la factura pagada), `Serie`, `Folio`, `MonedaDR`, `EquivalenciaDR`, `NumParcialidad`, `ImpSaldoAnt`, `ImpPagado`, `ImpSaldoInsoluto` y `ObjetoImpDR` |

Quedan fuera las cuentas bancarias, la cadena del SPEI y los impuestos por documento y por pago.

### 4.4 Carta Porte 3.1

`ComprobanteData.CartaPorte`, espacio de nombres `DanmaobCfdi.Core.Reading.CartaPorte31`. Nulo si el CFDI no trae el complemento.

| Tipo | Contenido |
| --- | --- |
| `CartaPorteData` | `Version`, `IdCCP`, `TranspInternac`, `EntradaSalidaMerc`, `PaisOrigenDestino`, `ViaEntradaSalida`, `TotalDistRec`, la lista `Ubicaciones`, `Mercancias` y la lista `FiguraTransporte` |
| `UbicacionData` | `TipoUbicacion`, `IDUbicacion`, `RFCRemitenteDestinatario`, `NombreRemitenteDestinatario`, `NumRegIdTrib`, `ResidenciaFiscal`, `FechaHoraSalidaLlegada`, `DistanciaRecorrida` y `Domicilio` |
| `DomicilioData` | `Calle`, `NumeroExterior`, `NumeroInterior`, `Colonia`, `Localidad`, `Referencia`, `Municipio`, `Estado`, `Pais` y `CodigoPostal` (las claves tal como vienen) |
| `MercanciasData` | `PesoBrutoTotal`, `UnidadPeso`, `PesoNetoTotal`, `NumTotalMercancias`, la lista `Mercancias` y `Autotransporte` |
| `MercanciaData` | `BienesTransp`, `Descripcion`, `Cantidad`, `ClaveUnidad`, `Unidad`, `MaterialPeligroso` (`Sí` o `No`, como viene), `CveMaterialPeligroso`, `Embalaje`, `PesoEnKg`, `ValorMercancia`, `Moneda` y `FraccionArancelaria` |
| `AutotransporteData` | `PermSCT`, `NumPermisoSCT`, `IdentificacionVehicular`, `Seguros` y la lista `Remolques`. Nulo si la mercancía viaja por otro medio |
| `IdentificacionVehicularData` | `ConfigVehicular`, `PesoBrutoVehicular`, `PlacaVM` y `AnioModeloVM` |
| `SegurosData` | `AseguraRespCivil`, `PolizaRespCivil`, `AseguraMedAmbiente`, `PolizaMedAmbiente`, `AseguraCarga`, `PolizaCarga` y `PrimaSeguro` |
| `RemolqueData` | `SubTipoRem` y `Placa` |
| `TiposFiguraData` | `TipoFigura`, `RFCFigura`, `NumLicencia`, `NombreFigura`, `NumRegIdTribFigura`, `ResidenciaFiscalFigura`, la lista `PartesTransporte` y `Domicilio` |

Los datos de la unidad (`IdentificacionVehicular`, `Seguros`, `Remolques`) y del operador (`FiguraTransporte` con `TipoFigura` `01`) son los que el TMS puede usar para completar sus catálogos.

Quedan fuera: `RegimenesAduaneros`, los datos del Istmo, los datos de estación de las ubicaciones, la documentación aduanera, las guías, `CantidadTransporta`, `DetalleMercancia`, las dimensiones, los datos de COFEPRIS y el transporte marítimo, aéreo y ferroviario (una Carta Porte por esos medios se lee con `Autotransporte` en nulo).

### 4.5 Acuse de cancelación (`Kind == DocumentKind.CancellationAcknowledgment`)

`XmlReadResult.Cancelacion`, espacio de nombres `DanmaobCfdi.Core.Reading.Cancelacion`. Es el XML que entrega el PAC al cancelar (la respuesta de `fnCancelarXML20` de PAX), con raíz `Cancelacion` en `http://cancelacfd.sat.gob.mx`.

| Tipo | Contenido |
| --- | --- |
| `CancelacionData` | `RfcEmisor`, `Fecha` y la lista `Folios` |
| `FolioData` | `UUID`, `UUIDEstatus`, `UUIDdescripcion` y `UUIDfecha` |

- A todos los valores se les quitan los espacios sobrantes, que PAX suele incluir.
- **El componente no interpreta los códigos de estatus.** `UUIDEstatus` y `UUIDdescripcion` llegan tal como vienen; el producto decide cuáles da por cancelado (por ejemplo, en la documentación de PAX el `202` significa aceptación en proceso, no cancelado).
- La firma del acuse se ignora y no se verifica.
- Si el cliente sube la respuesta SOAP completa del PAC, y no solo el acuse, el resultado es `Unrecognized`.
- La verificación del acuse es propia del componente y no oficial: no existe un esquema del SAT para él. Se construyó con la forma del ejemplo de PAX; todavía falta confirmarla con un acuse real.

## 5. Lo que no hace la versión 1.0.0

- No emite, sella, timbra ni cancela comprobantes.
- No consulta catálogos ni entrega descripciones de claves.
- No verifica sellos ni consulta el estatus de un CFDI ante el SAT.
- No guarda comprobantes, XML ni PDF: eso es del producto.
- No lee Retenciones e información de pagos.
