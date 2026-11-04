# Registro de cambios de DANMAOB_CFDI

Una sección por versión entregada, de la más reciente a la más antigua (ADR-02, sección 8). Cada versión entregada tiene su etiqueta `vX.Y.Z` en este repositorio, y la copia de cada producto se importa o se reemplaza desde esa etiqueta con `tools/copia/reemplazar-copia.sh`. El contrato público de cada versión está en `src/GUIA-DE-USO.md`, que viaja con la copia.

## [1.0.0] — 2026-11-03 — `CFDI_Lectura`

Primera versión entregada. Su contrato público es solo la lectura y validación estructural de XML de terceros.

### Agregado

- Registro del componente con `AddDanmaobCfdiCore` y lectura con `IThirdPartyXmlReader.Read(byte[])`, que devuelve `XmlReadResult` con cinco estados (`Read`, `Unsupported`, `Unrecognized`, `Invalid` y `Rejected`) y nunca lanza una excepción por el contenido del documento.
- Carga segura del XML: sin DTD ni entidades, solo UTF-8, hasta 10 MB y 64 niveles de profundidad.
- Detección del tipo y la versión del documento, y validación estructural contra los esquemas oficiales del SAT incluidos en el componente, sin red.
- Lectura de CFDI 4.0 (comprobante, emisor, receptor, conceptos, impuestos y CFDI relacionados) con Timbre Fiscal Digital 1.1, complemento de Pagos 2.0 y complemento Carta Porte 3.1 (ubicaciones, mercancías, autotransporte y figuras de transporte).
- Lectura del acuse de cancelación en XML, con una verificación propia basada en la forma del ejemplo de PAX.
- Los complementos fuera del alcance de la lectura, incluidos los de impuestos locales y leyendas fiscales, se reportan en `UnreadComplements`.
- `ComponentVersion.Current` con la versión del componente.
- Guía de uso del contrato público (`src/GUIA-DE-USO.md`) y procedimiento de importación y reemplazo de la copia (`tools/copia/`, `docs/procedimientos/reemplazo-de-copia.md`).

### Incompatible

Nada: es la primera versión.

### Migraciones

La copia incluye las seis migraciones del esquema `sat` con las tablas de 57 catálogos, todavía sin datos: `InitialSatSchema`, `Cfdi40SimpleCatalogs`, `Cfdi40OwnStructureCatalogs`, `Cfdi40GeographicCatalogs`, `CartaPorte31SimpleCatalogs` y `CartaPorte31OwnStructureCatalogs`. Los catálogos no forman parte del contrato de esta versión: un producto que solo usa la lectura no necesita aplicarlas todavía.

### Catálogos

Ninguno con datos en esta versión.

### Paquetes

- `Microsoft.EntityFrameworkCore.SqlServer` y `Microsoft.EntityFrameworkCore.Design` 10.0.12.
- `Microsoft.Extensions.DependencyInjection.Abstractions` 10.0.12.
- `System.Security.Cryptography.Pkcs` 10.0.13, fijado como referencia directa en `DanmaobCfdi.Catalogs` porque las versiones anteriores tienen vulnerabilidades conocidas.

### Pendiente

- Confirmar la forma del acuse de cancelación con un acuse real del primer cliente.
- Probar la lectura con XML reales del cliente cuando el producto integre esta versión.
- La primera importación en DANMAOB_TMS se hace cuando exista su módulo de facturación.
