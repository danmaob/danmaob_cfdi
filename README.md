# DANMAOB_CFDI

Componente fiscal de DANMAOB: una biblioteca de clases .NET 10 que genera, sella, timbra, cancela y lee comprobantes fiscales digitales (CFDI) de México, y que mantiene los catálogos del SAT en una base de datos SQL Server.

No es una API ni un servicio. Se incluye como copia en la solución de cada producto que necesita facturar: DANMAOB_TMS y DANMAOB_ERP. Este repositorio es el original único; las copias nunca se editan dentro de los productos.

## Qué incluye y qué no

| Incluye | No incluye |
| --- | --- |
| CFDI 4.0, Pagos 2.0, Carta Porte 3.1, impuestos locales, leyendas fiscales y Retenciones 2.0 | API web, autenticación o interfaz de usuario |
| Generación del XML, cadena original y sellado | Custodia del Certificado de Sello Digital (CSD) y de las credenciales del PAC: son del producto |
| Timbrado y cancelación con PAX, detrás de una interfaz | Almacenamiento de comprobantes, XML o PDF |
| Lectura y validación estructural de XML de terceros | Empresas, clientes, tenants o cualquier dato de negocio |
| Catálogos del SAT en el esquema `sat`: modelo, migraciones, carga y consulta | Conteo y cobro de timbres |

## Estructura

| Ruta | Contenido |
| --- | --- |
| `DanmaobCfdi.slnx` | Solución |
| `global.json` | Versión del SDK de .NET |
| `src/` | **Unidad de copia**: lo único que viaja a cada producto, con su propia configuración de compilación |
| `src/DanmaobCfdi.Core/` | Núcleo fiscal sin base de datos ni red: recursos oficiales del SAT, modelos, lectura de XML, cadena original, sellado, cálculos e interfaces de catálogos y del PAC |
| `src/DanmaobCfdi.Catalogs/` | Catálogos del SAT: `DbContext` del esquema `sat`, migraciones y consulta |
| `src/DanmaobCfdi.Pax/` | Adaptador del PAC (PAX) |
| `tests/DanmaobCfdi.Tests/` | Pruebas unitarias, de ejecución real y de arquitectura |
| `docs/adr/` | Decisiones de arquitectura |
| `tools/` | Herramientas de desarrollo (por ejemplo, la conversión de los catálogos del SAT); no viajan con la copia |

La versión del componente está en la propiedad `Version` de `src/Directory.Build.props`.

## Requisitos

- SDK de .NET 10.0.400 (lo fija `global.json`).
- Para compilar y probar no se necesita red ni base de datos.
- SQL Server solo se necesita para aplicar las migraciones del esquema `sat`.

## Compilar y probar

Desde la raíz del repositorio:

```
dotnet build DanmaobCfdi.slnx
dotnet test DanmaobCfdi.slnx
```

Las pruebas nunca se conectan a la base de datos ni a un PAC. Las pruebas contra el ambiente de pruebas del PAC se ejecutan aparte y solo con credenciales que se pasan por variables de entorno.

## Reglas de dependencias

`DanmaobCfdi.Core` no depende de Entity Framework Core, de SQL Server ni de los clientes del PAC. `DanmaobCfdi.Catalogs` y `DanmaobCfdi.Pax` dependen de `Core` y nunca entre sí. Las pruebas de `tests/DanmaobCfdi.Tests/Architecture/` fallan si alguna regla se rompe (ADR-01).

## Distribución por copia

- Cada entrega es una etiqueta `vX.Y.Z` en `main`, con versión semántica (ADR-02).
- El producto recibe el contenido de `src/` de esa etiqueta en una carpeta `DanmaobCfdi/` junto a su solución, y agrega a su solución los proyectos que use.
- Una versión con sufijo `-dev` nunca se entrega a un producto.
- Las migraciones del esquema `sat`, incluidos los datos de catálogos, viajan con la copia. El producto las aplica como paso explícito de su despliegue; el componente nunca las aplica al arrancar.

## Archivos protegidos

- `src/DanmaobCfdi.Core/SatResources/`: esquemas y hojas de estilo oficiales del SAT, idénticos byte por byte a los publicados. Nunca se editan; se reemplazan completos con cada publicación.
- `src/DanmaobCfdi.Catalogs/SatData/`: datos de catálogos generados por la herramienta de `tools/`. Nunca se editan a mano.
- `tests/DanmaobCfdi.Tests/Fixtures/`: archivos de referencia de las pruebas y CSD públicos de pruebas del SAT.

Ningún certificado, llave, contraseña ni credencial real entra al repositorio.

## Decisiones de arquitectura

| ADR | Tema |
| --- | --- |
| [ADR-01](docs/adr/ADR-01-estructura-solucion.md) | Estructura de la solución |
| [ADR-02](docs/adr/ADR-02-versionado.md) | Versionado del original y de las copias |
| [ADR-03](docs/adr/ADR-03-recursos-sat.md) | Recursos oficiales del SAT y resolución local de XSD y XSLT |
| [ADR-04](docs/adr/ADR-04-estrategia-pruebas.md) | Estrategia de pruebas |
| [ADR-05](docs/adr/ADR-05-lectura-xml-terceros.md) | Lectura y validación estructural de XML de terceros |
| [ADR-06](docs/adr/ADR-06-catalogos-modelo-datos.md) | Catálogos: modelo de datos, migraciones y acceso |
| [ADR-07](docs/adr/ADR-07-catalogos-carga.md) | Catálogos: proceso de carga desde los archivos del SAT |
