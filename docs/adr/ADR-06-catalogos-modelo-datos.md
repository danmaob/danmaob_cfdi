# ADR-06 — Catálogos: modelo de datos, migraciones y acceso

**Estado:** Aceptado
**Fecha:** 19 de octubre de 2026
**Historia:** SP-03-1 (épica E03 — Catálogos: modelo y consulta)
**Requerimientos:** P05; D-9; D-16; D-17; D-18; D-19; D-20; D-24; D-25; D-26; D-27; §4
**Depende de:** ADR-01 (estructura de la solución), ADR-02 (versionado) y ADR-04 (estrategia de pruebas)

## Contexto

Los catálogos del SAT son la única persistencia del componente. Viven en SQL Server, en el esquema `sat` de la base compartida de cada producto, con un `DbContext`, migraciones e historial propios del componente (D-9). No tienen columna de empresa, y el producto no mapea ni modifica sus tablas (instrucciones, sección 4).

CFDIAPI40 tenía 37 tablas de catálogo con estos defectos (informe de revisión, secciones 4.5 y 4.6):

- llaves sustitutas `int` y un campo `Activo` mantenido a mano, sin vigencia ni versión del catálogo;
- una validación que solo comprobaba que la clave existiera en algún renglón, sin vigencia ni relaciones (el municipio `001` existe en casi todos los estados), y la de fracción arancelaria desactivada;
- `c_RegimenFiscal` con un solo indicador de persona física, que no representa regímenes que aplican a ambas;
- `c_UsoCFDI` sin los regímenes fiscales del receptor permitidos;
- un `DbContext` estático compartido entre peticiones, que cargaba catálogos completos en memoria y los recorría uno por uno.

DANMAOB_CFDI tiene **61 catálogos**: 27 de CFDI 4.0, 29 de Carta Porte 3.1, `c_FraccionArancelaria` y 4 de Retenciones 2.0 (informe, sección 6.3). Ya está decidido:

| Decisión | Contenido |
| --- | --- |
| D-16 | Los datos de catálogos se actualizan con la versión del componente |
| D-17 y D-18 | Se carga `c_FraccionArancelaria`; se eliminan cuatro tablas que no son del SAT o que ya no se usan |
| D-19 y D-25 | `c_MotivoCancelacion` y `c_TipoCadenaPago` son tablas con datos fijos versionados |
| D-20 | Los 4 catálogos del comprobante base de Retenciones, en tablas propias |
| D-24 | Colonia, localidad y municipio se cargan una sola vez, desde el Excel de CFDI |
| D-26 | Consulta por clave y por búsqueda con límite; los catálogos pequeños se pueden devolver completos |
| D-27 | La emisión no se bloquea por `c_NumPedimentoAduana` ni `c_PatenteAduanal` |

Los catálogos más grandes son `c_Colonia` (145 mil registros), `c_CodigoPostal` (96 mil), `c_NumPedimentoAduana` (59 mil), `c_ClaveProdServ` (53 mil) y `c_ClaveProdServCP` (49 mil). Los datos del SAT no son consistentes entre catálogos, y algunos traen vigencias que no son fechas ("Por definir").

## Decisión

### 1. `DbContext`, esquema e historial

- Un solo contexto, `SatDbContext`, en `DanmaobCfdi.Catalogs`, con todas las tablas en el esquema `sat`.
- El historial de migraciones vive en `sat.__EFMigrationsHistory`. Nunca choca con el historial del producto, que vive en `dbo`.
- El producto pasa la cadena de conexión al registrar el componente con `AddDanmaobCfdiCatalogs`. El componente nunca lee configuración por su cuenta.
- Una fábrica de diseño permite que `dotnet ef` construya el contexto sin proyecto de arranque. Toma la cadena de conexión de la variable de entorno `DANMAOB_CFDI_SAT_CONNECTION`, que se pasa en el mismo comando y solo para ese comando (instrucciones, sección 5.1). Así las migraciones se generan en el original y el producto las aplica igual (ADR-02, sección 6).
- El contexto es de solo lectura: toda consulta es sin seguimiento de cambios (`AsNoTracking`). Los datos solo cambian por migraciones.
- El contexto se registra con duración por solicitud (`Scoped`). Nunca es estático.

### 2. Nombres

| Elemento | Regla | Ejemplo |
| --- | --- | --- |
| Tabla | Nombre exacto del catálogo del SAT, en el esquema `sat` | `sat.c_FormaPago` |
| Clase de entidad | El mismo nombre sin el prefijo `c_` | `FormaPago` |
| Carpeta y namespace de la entidad | Por origen del catálogo: `Cfdi40`, `CartaPorte31`, `ComercioExterior` o `Retenciones20` | `DanmaobCfdi.Catalogs.Cfdi40` |
| Columnas comunes | Iguales en todas las tablas | `Clave`, `Descripcion`, `FechaInicioVigencia`, `FechaFinVigencia` |
| Columnas propias | En español, con mayúscula inicial en cada palabra y el vocabulario del SAT | `AplicaPersonaFisica` |

Los nombres uniformes de las columnas comunes permiten una consulta genérica para cualquier catálogo.

`Descripcion` admite nulo. Es obligatoria en cada catálogo cuyo Excel la trae, y queda en nulo solo en los que no traen descripción: `c_TipoFactor`, `c_PatenteAduanal` y `c_CodigoPostal`. Nunca se rellena con otro dato, como la clave. En `c_ClaveUnidad`, `Descripcion` guarda el nombre de la unidad, y la descripción propia del SAT va en otra columna.

### 3. Llaves

- La llave principal es la **clave del SAT**, como texto `nvarchar` con la longitud de su XSD. Conserva los ceros a la izquierda.
- Llaves compuestas:

| Catálogo | Llave, en este orden |
| --- | --- |
| `c_Colonia` | Código postal y clave |
| `c_Municipio` | Estado y clave |
| `c_Localidad` | Estado y clave |
| `c_Estado` | País y clave |

- El padre va primero en la llave, para que la misma llave sirva para buscar los hijos de un padre (por ejemplo, las colonias de un código postal).
- Un catálogo **sin clave del SAT** lleva una llave numérica propia y un índice único sobre las columnas que definen cada renglón. En CFDI 4.0 son dos, confirmados con los datos del Excel: `c_TasaOCuota` y `c_NumPedimentoAduana`. Los de Carta Porte 3.1 se confirman en TS-03-3.

### 4. Vigencia

- `FechaInicioVigencia` y `FechaFinVigencia` son de tipo `date` y admiten nulo.
- Inicio nulo: vigente desde siempre. Cubre los catálogos sin fechas, como `c_Pais` y `c_Colonia`.
- Fin nulo: sin fecha de fin. Cubre también los valores que no son fecha, como "Por definir".
- Una clave está **vigente** en una fecha si el inicio es nulo o menor o igual que la fecha, y el fin es nulo o mayor o igual. El fin es **inclusivo**: es el último día válido.
- La fecha que se compara es la del comprobante, sin hora.

### 5. Datos con estructura propia

| Catálogo | Representación |
| --- | --- |
| `c_RegimenFiscal` | Dos columnas de sí o no: `AplicaPersonaFisica` y `AplicaPersonaMoral` |
| `c_UsoCFDI` | Las mismas dos columnas, más los regímenes fiscales del receptor permitidos en una tabla hija, `sat.c_UsoCFDI_RegimenFiscalReceptor`, con un renglón por uso y régimen, en lugar del texto separado por comas del Excel |
| `c_FormaPago` | Sus columnas de reglas y patrones, como texto tal como vienen en el Excel. Interpretarlas es trabajo de la validación (SP-05-1 y E07) |
| `c_CodigoPostal` | Estado, municipio, localidad, estímulo de franja fronteriza y datos del huso horario |
| `c_TasaOCuota` | Tipo (rango o fijo), valor mínimo y valor máximo en `decimal(18,6)`, impuesto, factor, traslado y retención |
| `c_ClaveUnidadPeso` | La columna de bandera que sustituye a la tabla `CAT_CCP_TipoBandera` (D-18) |

Regla general para las demás columnas del Excel:

- Una columna que solo contiene "Sí" y "No" se guarda como `bit`.
- Cualquier otra se guarda como texto tal como viene, incluido "Opcional".

### 6. Tabla de versiones cargadas

`sat.CatalogoVersion` tiene un renglón por catálogo:

| Columna | Contenido |
| --- | --- |
| `Catalogo` | Nombre del catálogo del SAT. Es la llave |
| `VersionSat`, `RevisionSat` y `FechaPublicacionSat` | Los metadatos de la publicación, tomados del Excel. Admiten nulo cuando el Excel no los trae |
| `VersionComponente` | La versión del componente que cargó los datos (ADR-02) |
| `FechaAplicacionUtc` | Fecha y hora de la carga, en UTC |

Cada carga reemplaza el renglón de su catálogo. El historial completo queda en el historial de migraciones. El llenado lo define SP-04-1 y lo implementa TS-04-3.

### 7. Contrato de consulta

La interfaz de consulta vive en `DanmaobCfdi.Core` (ADR-01, sección 2) y su implementación en `DanmaobCfdi.Catalogs`. Todos sus métodos son asíncronos y aceptan cancelación, porque consultan la base de datos.

**Consulta genérica, para cualquiera de los 61 catálogos.** El catálogo se identifica con un valor de una enumeración con los 61 catálogos, que además marca cuáles son pequeños.

| Operación | Comportamiento |
| --- | --- |
| Por clave | Recibe la clave, y la clave padre si la llave es compuesta. Devuelve clave, descripción y vigencia, o nada si no existe |
| Búsqueda con límite | Ver sección 9. Nunca devuelve más resultados que el límite |
| Vigencia a una fecha | Distingue tres resultados: vigente, no vigente y no existe |
| Lista completa | Solo para catálogos marcados como pequeños (hasta 1,000 registros). Pedirla para un catálogo grande es un error de programación |

**Consulta específica, solo para los catálogos de la sección 5.** Métodos tipados que devuelven sus datos propios: indicadores de persona física y moral, regímenes del receptor permitidos, reglas de la forma de pago, datos del código postal y rangos de tasa o cuota.

- Las pruebas de quien usa esta interfaz usan un doble escrito a mano, nunca la base de datos (ADR-04, sección 7).
- Los nombres exactos de la interfaz, la enumeración y los tipos de resultado los fijan TS-03-5 y TS-03-6.

### 8. Relaciones entre catálogos

El componente ofrece estas verificaciones:

| Verificación | Cómo |
| --- | --- |
| El código postal existe y su estado coincide | Por la llave de `c_CodigoPostal` |
| El municipio corresponde al código postal y al estado | Por las columnas de `c_CodigoPostal` y la llave de `c_Municipio` |
| La localidad corresponde al código postal y al estado | Por las columnas de `c_CodigoPostal` y la llave de `c_Localidad` |
| La colonia existe para ese código postal | Por la llave compuesta de `c_Colonia` |

- Cuáles se aplican antes de timbrar depende de la matriz de errores de Carta Porte 3.1 (TK-07-1, D-24) y se fija en E07. La emisión no se bloquea por `c_NumPedimentoAduana` ni `c_PatenteAduanal` (D-27).
- **No hay llaves foráneas entre tablas de catálogos.** Los datos del SAT no garantizan consistencia entre catálogos, y una llave foránea haría fallar la carga completa por un solo renglón huérfano. Las relaciones se verifican con consultas, y la carga reporta los huérfanos (SP-04-1).
- La única llave foránea es la de la tabla hija `sat.c_UsoCFDI_RegimenFiscalReceptor` hacia `sat.c_UsoCFDI`, porque ambas se cargan juntas desde el mismo renglón del Excel.

### 9. Búsqueda e índices

**Búsqueda:**

- Busca por **prefijo de la clave** o por **texto contenido en la descripción**.
- El texto de búsqueda tiene al menos 3 caracteres.
- El límite predeterminado es de 20 resultados y el máximo de 50.
- En un catálogo con llave compuesta se puede acotar por la clave padre (por ejemplo, colonias de un código postal).

**Comparación de descripciones:**

- La columna `Descripcion` declara su propia intercalación, `Latin1_General_100_CI_AI`, que no distingue mayúsculas ni acentos. Así "credito" encuentra "crédito", sin depender de la intercalación de la base del producto.

**Índices:**

- No se usa la búsqueda de texto completo de SQL Server: no viene en la imagen estándar de Docker y agregaría infraestructura sin necesidad demostrada.
- La búsqueda por texto contenido recorre la tabla con límite. En los catálogos más grandes (145 mil colonias) es un costo aceptable.
- Las llaves principales cubren la búsqueda por clave, por prefijo de clave y por relaciones, porque su orden pone primero al padre.
- No se crean otros índices sin una medición que los justifique. TS-03-4 confirma con los planes de consulta que las llaves principales cubren esas búsquedas.

## Fuera del alcance de este ADR

- Cómo se leen los Excel, cómo se convierten en datos versionados y cómo se reportan los huérfanos: SP-04-1.
- Qué relaciones y reglas se aplican antes de timbrar: E07, con la matriz de errores (TK-07-1).
- Los catálogos de los complementos de Retenciones: TK-12-1.

## Alternativas consideradas

| Alternativa | Por qué no se eligió |
| --- | --- |
| Llaves sustitutas `int` y campo `Activo`, como CFDIAPI40 | La clave del SAT ya es única, y `Activo` a mano no expresa la vigencia |
| Historial de migraciones compartido con el producto | Las migraciones del componente y las del producto chocarían |
| Prefijo `c_` en las clases de entidad | Se aparta de las convenciones de nombres de C# sin ganar claridad: la tabla conserva el nombre del SAT |
| Llaves foráneas entre todos los catálogos | Un renglón huérfano del SAT haría fallar la carga completa |
| Regímenes del receptor como texto separado por comas | No se puede consultar ni validar sin interpretar el texto en cada uso |
| Un método tipado por cada uno de los 61 catálogos | Una interfaz enorme. La consulta genérica cubre la clave, la descripción y la vigencia |
| Catálogos cargados en memoria, como CFDIAPI40 | Cientos de miles de registros por proceso. La base responde con límite |
| Búsqueda de texto completo de SQL Server | Infraestructura adicional que la imagen estándar de Docker no trae |
| Intercalación de la base del producto | La búsqueda se comportaría distinto en cada producto |
| Vigencia sin fechas, solo con el contenido del Excel vigente | No se podría validar un comprobante contra la fecha en que se emitió |

## Consecuencias

**Lo que se facilita:**

- Validar una clave con su vigencia a la fecha del comprobante, y las relaciones de un domicilio, sin cargar catálogos en memoria.
- Cotejar cada tabla con su Excel y su XSD por el nombre.
- Que el producto y el componente compartan la base sin compartir historial de migraciones.
- Probar todo lo que usa catálogos sin base de datos.

**Lo que exige cuidado:**

- La intercalación de `Descripcion` se declara en cada tabla; una migración que la omita cambia el comportamiento de la búsqueda.
- Sin llaves foráneas, la consistencia entre catálogos depende de la verificación de la carga (SP-04-1).
- La lista de catálogos sin clave del SAT y los catálogos pequeños se confirman con la estructura de cada Excel.
- La variable `DANMAOB_CFDI_SAT_CONNECTION` nunca se deja cargada en la Terminal: se pasa solo en el comando que la usa.

**Trabajo desbloqueado:**

| Elemento | Qué toma de este ADR |
| --- | --- |
| TS-03-1 | Contexto, esquema, historial y tabla de versiones (secciones 1 y 6) |
| TS-03-2 y TS-03-3 | Nombres, llaves, vigencia y datos con estructura propia (secciones 2 a 5) |
| TS-03-4 | Confirmación de índices (sección 9) |
| TS-03-5 | Contrato de consulta y búsqueda (secciones 7 y 9) |
| TS-03-6 | Vigencia a una fecha (secciones 4 y 7) |
| TS-03-7 | Relaciones (sección 8) |
| SP-04-1 | Tabla de versiones y reporte de huérfanos (secciones 6 y 8) |

## Historial de cambios

| Fecha | Cambio |
| --- | --- |
| 21 de octubre de 2026 | Precisión por el ADR-09 (sección 6): `c_Moneda` se agrega a la sección 5, con `Decimales` como entero y `PorcentajeVariacion` como `decimal`, y con una consulta específica que devuelve esos datos. El servicio de cálculos recibe los decimales de la moneda como parámetro; el servicio de emisión los obtiene con esa consulta |
| 27 de octubre de 2026 | Precisión por TS-03-2, con los datos del Excel de CFDI 4.0: `Descripcion` admite nulo y solo queda en nulo en `c_TipoFactor`, `c_PatenteAduanal` y `c_CodigoPostal`, que no traen descripción (sección 2); en `c_ClaveUnidad` guarda el nombre de la unidad. Se confirman los dos catálogos sin clave del SAT de CFDI 4.0, `c_TasaOCuota` y `c_NumPedimentoAduana` (sección 3) |
| 29 de octubre de 2026 | Precisión por TS-03-2B, con los datos del Excel: `c_TasaOCuota` y la tabla hija `c_UsoCFDI_RegimenFiscalReceptor` no heredan las columnas comunes (no tienen clave del SAT ni descripción); el índice único de `c_TasaOCuota` se declara sin filtro, para que los renglones fijos (sin valor mínimo) también cuenten en la unicidad; `c_TipoDeComprobante` guarda `ValorMaximo` en `decimal(24,6)`, y el renglón sin clave del Excel es la continuación de Nómina (`NS` y `NdS` con el mismo valor), así que `N` guarda ese único valor; las columnas de `c_FormaPago` que mezclan `Sí`, `No` y `Opcional` van como texto, y `c_Impuesto` guarda `Retencion` y `Traslado` como `bit` (el Excel escribe `Si` sin acento) |
| 29 de octubre de 2026 | Precisión por TS-03-2C, con los datos del Excel: los catálogos con llave compuesta (`c_Estado` con `Pais`; `c_Municipio` y `c_Localidad` con `Estado`; `c_Colonia` con `CodigoPostal`) mapean sus columnas comunes con `SatCatalogConfiguration.ConfigureColumns`, que no fija llave, y declaran su llave de dos columnas, para que `Clave` sola nunca quede como llave alterna; `c_CodigoPostal` guarda estado, municipio y localidad sin llave foránea, el estímulo de franja fronteriza como `tinyint` (0, 1 y 2) y los datos de horario de verano e invierno; `c_ClaveProdServ` guarda el mismo estímulo como `tinyint` y `PalabrasSimilares` con la intercalación de las descripciones; `c_ClaveUnidad` guarda la descripción propia del SAT en `DescripcionUnidad`; `c_NumPedimentoAduana` lleva `Id` con identidad e índice único sobre aduana, patente y ejercicio; `c_Pais` y `c_Municipio` llevan descripción de 100 aunque su máximo es 49 |
