# ADR-03 — Recursos oficiales del SAT y resolución local de XSD y XSLT

**Estado:** Aceptado
**Fecha:** 17 de octubre de 2026
**Historia:** SP-01-3 (épica E01 — Fundamentos del componente)
**Requerimientos:** P03; D-12; D-15; §3.5; §4
**Depende de:** ADR-01 (estructura de la solución) y ADR-02 (versionado)

## Contexto

El componente necesita los esquemas (XSD) y las hojas de estilo (XSLT) oficiales del SAT para dos cosas: validar la estructura de los XML y calcular la cadena original. Las instrucciones exigen que ninguna inclusión se resuelva por internet y que la XSLT se cargue sin scripts y en un solo lugar (sección 4).

CFDIAPI40 hacía lo contrario:

- cargaba la XSLT con scripts y `document()` habilitados y en modo de depuración;
- usaba un resolutor que seguía las inclusiones por internet;
- tomaba la XSLT de dos rutas distintas.

El juego vigente se descargó del portal del SAT el 16 de octubre de 2026 (D-15) e incluye la XSLT principal de CFDI 4.0 completa, con todas sus hojas (D-12). La evidencia del 17 de octubre de 2026 sobre `/Users/luisortiz/Desarrollo/DANMAOB/DANMAOB_CFDI/XSD` muestra:

| Hecho | Detalle |
| --- | --- |
| Archivos | 60, en una sola carpeta, sin subcarpetas |
| Tamaños mayores | `catCFDI.xsd` 5.7 MB, `catCartaPorte.xsd` 2.3 MB, `catComExt.xsd` 1.0 MB |
| Inclusiones de XSLT | `cadenaoriginal_4_0.xslt` incluye 33 hojas; `retenciones.xslt` incluye 13. Comparten `utilerias.xslt` |
| Importaciones de XSD | `cfdv40.xsd`, `Pagos20.xsd`, `CartaPorte31.xsd`, `retencionpagov2.xsd` y `TimbreFiscalDigitalv11.xsd` importan 6 XSD de catálogos y tipos de datos |
| Direcciones | Todas son `http://www.sat.gob.mx/sitio_internet/...` o `http://www.sat.gob.mx/esquemas/...` |
| Archivos con dirección conocida | 51 aparecen en alguna inclusión o importación |
| Archivos de entrada | 9 que nadie incluye: `cadenaoriginal_4_0.xslt`, `retenciones.xslt`, `cfdv40.xsd`, `Pagos20.xsd`, `CartaPorte31.xsd`, `implocal.xsd`, `leyendasFisc.xsd`, `TimbreFiscalDigitalv11.xsd` y `retencionpagov2.xsd` |
| Versión de XSLT | Las 47 hojas declaran `version="2.0"`. Ninguna usa `document()`, scripts ni construcciones exclusivas de XSLT 2.0 |

Las hojas de Retenciones 2.0 incluyen sus complementos desde rutas `retencionpago/1/...`. Así los publica el SAT.

El procesador de XSLT de .NET (`XslCompiledTransform`) solo implementa XSLT 1.0. Ante una hoja que declara una versión mayor trabaja en modo de compatibilidad hacia adelante. Como las hojas solo usan funciones de XSLT 1.0, deben compilar y producir la cadena correcta.

## Decisión

### 1. Archivos oficiales intactos y resolutor de direcciones

Los archivos oficiales se guardan **idénticos byte por byte** a los publicados por el SAT. Nunca se edita una ruta dentro de ellos.

El componente usa un resolutor propio que convierte cada dirección oficial en su recurso local. El mismo resolutor atiende las inclusiones de las XSLT y las importaciones de los XSD. Si recibe una dirección que no corresponde a un recurso local, responde con un error. Nunca accede a la red.

### 2. Ubicación de cada archivo: la ruta de su dirección oficial

Dentro de `src/DanmaobCfdi.Core/SatResources/`, cada archivo vive en la ruta de su dirección oficial, sin el esquema ni el dominio:

| Dirección oficial | Ruta local dentro de `SatResources/` |
| --- | --- |
| `http://www.sat.gob.mx/sitio_internet/cfd/catalogos/catCFDI.xsd` | `sitio_internet/cfd/catalogos/catCFDI.xsd` |
| `http://www.sat.gob.mx/sitio_internet/cfd/tipoDatos/tdCFDI/tdCFDI.xsd` | `sitio_internet/cfd/tipoDatos/tdCFDI/tdCFDI.xsd` |
| `http://www.sat.gob.mx/sitio_internet/cfd/2/cadenaoriginal_2_0/utilerias.xslt` | `sitio_internet/cfd/2/cadenaoriginal_2_0/utilerias.xslt` |
| `http://www.sat.gob.mx/esquemas/retencionpago/1/dividendos/dividendos.xslt` | `esquemas/retencionpago/1/dividendos/dividendos.xslt` |

Regla del resolutor:

1. Acepta solo direcciones con esquema `http` o `https` y dominio `www.sat.gob.mx`, sin distinguir mayúsculas en el dominio.
2. Quita el esquema y el dominio, y la ruta restante es el nombre del recurso.
3. Cualquier otra dirección, o una ruta sin recurso, es un error.

No hay tabla de equivalencias que mantener: agregar un archivo es ponerlo en la ruta de su dirección.

Los 9 archivos de entrada van en la ruta de su dirección oficial publicada por el SAT. Las rutas esperadas son estas; TS-01-4 las confirma contra el portal del SAT antes de copiar:

| Archivo | Ruta esperada dentro de `SatResources/` |
| --- | --- |
| `cadenaoriginal_4_0.xslt` | `sitio_internet/cfd/4/cadenaoriginal_4_0/cadenaoriginal_4_0.xslt` |
| `cfdv40.xsd` | `sitio_internet/cfd/4/cfdv40.xsd` |
| `Pagos20.xsd` | `sitio_internet/cfd/Pagos/Pagos20.xsd` |
| `CartaPorte31.xsd` | `sitio_internet/cfd/CartaPorte/CartaPorte31.xsd` |
| `implocal.xsd` | `sitio_internet/cfd/implocal/implocal.xsd` |
| `leyendasFisc.xsd` | `sitio_internet/cfd/leyendasFiscales/leyendasFisc.xsd` |
| `TimbreFiscalDigitalv11.xsd` | `sitio_internet/cfd/TimbreFiscalDigital/TimbreFiscalDigitalv11.xsd` |
| `retencionpagov2.xsd` | `esquemas/retencionpago/2/retencionpagov2.xsd` |
| `retenciones.xslt` | `esquemas/retencionpago/2/retenciones.xslt` |

Como ninguna inclusión nombra a estos 9 archivos, una corrección de su ruta no afecta la resolución de los demás.

### 3. Recursos incrustados en el ensamblado

Los archivos `.xsd` y `.xslt` de `SatResources/` se incrustan como recursos de `DanmaobCfdi.Core`. Así la copia funciona en cualquier producto sin depender de carpetas de salida, de la publicación ni del directorio de trabajo, y los archivos no se pueden alterar en ejecución.

El ensamblado crece unos 11 MB, sobre todo por `catCFDI.xsd`. Es un costo aceptable.

MSBuild altera los nombres de recurso cuando una carpeta empieza con dígito (por ejemplo, `cfd/4/`). Por eso cada recurso lleva un **nombre lógico explícito igual a su ruta relativa dentro de `SatResources/`**, con `/` como separador (`sitio_internet/cfd/4/cfdv40.xsd`). El resolutor busca el recurso por ese nombre. La forma exacta en el `.csproj` la fija TS-01-5.

`SHA256SUMS` (sección 5) no se incrusta.

### 4. Un solo juego vigente

El repositorio contiene **un solo juego** de archivos del SAT: el vigente. La lectura solo acepta versiones vigentes (D-28), y mantener juegos anteriores en paralelo sería sobreingeniería.

- Cuando el SAT publica, los archivos se reemplazan en su lugar. La historia de Git conserva los anteriores.
- Se conservan los complementos de versiones anteriores que la XSLT principal incluye (por ejemplo `CartaPorte20.xslt` y `CartaPorte30.xslt`): sin ellos no compila.
- La convivencia de varios juegos solo se diseña si aparece una necesidad real, con un ADR nuevo.

### 5. Registro e integridad del juego

`SatResources/SHA256SUMS` registra el juego:

- La primera línea es `# juego SAT 2026-10-16`, con la fecha de descarga.
- Las demás líneas tienen el formato de `shasum -a 256`: el hash y la ruta relativa dentro de `SatResources/`, una por archivo.
- Las verificaciones ignoran las líneas que empiezan con `#`.

Lo genera el script de copia de Luis. Una prueba de TS-01-5 compara el contenido de cada recurso incrustado contra su hash, de modo que un archivo alterado o faltante rompe las pruebas.

### 6. Compilación única y configuración segura

| Recurso | Cómo se carga |
| --- | --- |
| XSLT de CFDI (`cadenaoriginal_4_0.xslt`) y de Retenciones (`retenciones.xslt`) | `XslCompiledTransform` con scripts y `document()` deshabilitados, sin depuración y con el resolutor del SAT para las inclusiones |
| Esquemas de CFDI | Un `XmlSchemaSet` con `cfdv40`, `Pagos20`, `CartaPorte31`, `implocal`, `leyendasFisc` y `TimbreFiscalDigitalv11`, con el resolutor del SAT para las importaciones |
| Esquemas de Retenciones | Un `XmlSchemaSet` aparte con `retencionpagov2` |

- Cada XSLT y cada conjunto de esquemas se compila **una sola vez**, la primera vez que se usa, de forma segura entre hilos, y se reutiliza durante toda la vida del proceso.
- Los archivos del SAT se leen sin procesar DTD.
- El resolutor del SAT solo se usa para esquemas y hojas de estilo. Los XML de terceros nunca lo usan: se leen sin resolutor y sin DTD. El detalle es de SP-02-1.

### 7. Protección de los archivos en Git

`src/.gitattributes` marca `DanmaobCfdi.Core/SatResources/**` como `-text -diff`:

- Git nunca convierte los fines de línea de esos archivos, ni en el original ni en la copia de cada producto.
- Git no muestra diferencias de archivos de varios MB.

Vive en `src/`, así que viaja con la copia (ADR-01, sección 4).

### 8. Incorporación inicial y actualizaciones del SAT

- **Incorporación inicial (TS-01-4):** un script de Luis copia los 60 archivos de `XSD/` a sus rutas, verifica que sean idénticos a los originales y genera `SHA256SUMS`. DEV JR nunca abre, edita ni copia estos archivos (instrucciones, sección 9.2).
- **Cada publicación nueva del SAT:**
  1. descarga desde el navegador y verificación con `file`;
  2. copia con el script de Luis, que regenera `SHA256SUMS`;
  3. pruebas reales de cadena original y de validación;
  4. versión MENOR del componente (ADR-02, sección 1).

## Fuera del alcance de este ADR

- El esquema para validar el acuse de cancelación, que el juego descargado no incluye: SP-02-1.
- Los XSD de los complementos de retenciones: TK-12-1.
- La XSLT de la cadena original del Timbre Fiscal Digital, que no se necesita por ahora.

## Alternativas consideradas

| Alternativa | Por qué no se eligió |
| --- | --- |
| Copia derivada con rutas locales | Obliga a editar 11 archivos oficiales y a repetir la edición en cada publicación del SAT. Además, ya no se puede comprobar que los archivos son idénticos a los oficiales |
| Resolutor que descarga por internet, con caché | Viola el principio "sin red" y hace que el componente dependa de la disponibilidad del portal del SAT |
| Tabla explícita de dirección a archivo | Otro artefacto que mantener. La ruta de la dirección ya identifica cada archivo sin ambigüedad |
| Archivos copiados a la carpeta de salida | Cada producto tendría que cuidar que los archivos lleguen a su salida y a su publicación, y podrían alterarse en ejecución |
| Recursos incrustados con los nombres que genera MSBuild | Los nombres se alteran cuando una carpeta empieza con dígito; el resolutor tendría que reproducir esas reglas |
| Varios juegos fechados en paralelo | Ningún consumidor lee versiones anteriores (D-28) |
| Un solo conjunto de esquemas para CFDI y Retenciones | La lectura de CFDI compilaría esquemas que no usa. Dos conjuntos mantienen cada uno pequeño |
| Compilar la XSLT y los esquemas en cada operación | Compilar `catCFDI.xsd` tarda; hacerlo en cada operación es un costo innecesario |
| Procesador de XSLT 2.0 de terceros | Dependencia innecesaria: ninguna hoja usa construcciones de XSLT 2.0 |

## Consecuencias

**Lo que se facilita:**

- Validar y calcular la cadena original nunca depende de internet.
- Comprobar que un archivo es idéntico al publicado por el SAT es una comparación directa.
- Incorporar una publicación nueva es reemplazar archivos en su ruta y regenerar `SHA256SUMS`, sin editar nada.
- Los productos no tienen que manejar archivos del SAT: vienen dentro del ensamblado.

**Lo que exige cuidado:**

- **XSLT 2.0:** que las hojas no usen construcciones de XSLT 2.0 se comprobó con una búsqueda, no con una ejecución. TS-01-5 y TS-01-6 ejecutan la transformación real y comparan la cadena original contra un resultado de referencia. Si alguna vez una hoja exigiera XSLT 2.0, esa prueba lo detectaría antes de cualquier timbrado.
- **Rutas de los archivos de entrada:** se confirman contra el portal del SAT en TS-01-4.
- **Primera carga:** la primera validación de un proceso tarda más mientras compila los esquemas; las siguientes reutilizan la compilación.
- **Archivos intocables:** ningún prompt ni script de corrección toca `SatResources/` (hard rule 2, ADR-01).

**Trabajo desbloqueado:**

- TS-01-4: recursos oficiales del SAT en el repositorio.
- TS-01-5: resolución local y compilación única de XSD y XSLT.
- SP-02-1 y SP-08-1, que parten de la validación estructural y de la cadena original locales.
