# ADR-07 — Catálogos: proceso de carga desde los archivos del SAT

**Estado:** Aceptado
**Fecha:** 19 de octubre de 2026
**Historia:** SP-04-1 (épica E04 — Catálogos: carga)
**Requerimientos:** P06; D-10; D-16; D-19; D-24; D-25; §9.2
**Depende de:** ADR-01 (estructura de la solución), ADR-02 (versionado), ADR-04 (estrategia de pruebas) y ADR-06 (catálogos: modelo de datos)

## Contexto

No existe respaldo de los catálogos de CFDIAPI40: se llenaron a mano y la base ya no existe. La carga parte de los Excel oficiales del SAT, guardados en `/Users/luisortiz/Desarrollo/DANMAOB/DANMAOB_CFDI/Docs` (D-10). Los datos de catálogos se actualizan con la versión del componente (D-16), y viajan como migraciones que el producto aplica (ADR-02, sección 6). La tabla `sat.CatalogoVersion` registra lo cargado (ADR-06, sección 6). DEV JR nunca abre los Excel (instrucciones, sección 9.2).

Los archivos son `.xls`, un formato binario antiguo que .NET no lee de fábrica. Luis tiene un entorno de Python aislado, `~/.venvs/sat-xls`, con `xlrd`.

La evidencia de la estructura de los Excel (`catalogos_estructura.txt`, sesión 2) muestra que no tienen un formato uniforme:

| Hallazgo | Ejemplos |
| --- | --- |
| Encabezados de una o de dos filas | `c_UsoCFDI` ("Aplica para tipo persona", con "Física" y "Moral" debajo), `c_TasaOCuota` (valor mínimo y máximo) y `c_CodigoPostal` (datos del huso horario) |
| Encabezados repetidos | `c_CodigoPostal` repite `Día_Inicio_Horario_Verano` en dos columnas |
| Catálogos partidos en varias hojas | `c_CodigoPostal` en 2 hojas, `c_Colonia` en 3 |
| Nombres de hoja irregulares | `'c_Estaciones '`, `' c_SubTipoRem'`, `'C_Colonia_1'` |
| Metadatos en filas distintas y con valores que no son fecha | La fila de versión, revisión y publicación está en la fila 3 o 4; varias "Fecha publicación" valen `0` |
| Claves que perdieron sus ceros a la izquierda | `c_FormaPago` guarda `1` en lugar de `01` |
| Renglones que no son datos | El texto al final de la primera hoja de `c_CodigoPostal`; el renglón con clave vacía de `c_TipoDeComprobante` |
| Datos con otra representación | `c_TasaOCuota` trae el impuesto por nombre (`IVA`, `IEPS`), no por clave (`002`); `c_UsoCFDI` trae los regímenes del receptor como lista separada por comas |
| Hojas que no se usan | Colonia, localidad y municipio del Excel de Carta Porte (D-24); 6 de las 10 hojas de Retenciones (D-20) |

Los catálogos más grandes suman unos 400 mil registros. `c_MotivoCancelacion` y `c_TipoCadenaPago` no tienen Excel: son datos fijos versionados (D-19 y D-25).

## Decisión

### 1. Los Excel se leen una sola vez, en el original

- Cada vez que el SAT publica, una herramienta de desarrollo convierte los Excel en **archivos de datos versionados** dentro del componente.
- Los productos aplican esos datos con migraciones (sección 6).
- Ningún producto lleva Excel ni una biblioteca que lea `.xls`.

### 2. La herramienta: un script de Python

- Vive en `Net/tools/sat-catalogs/`. No forma parte de la unidad de copia (ADR-01, sección 4).
- La escribe Claude y la ejecuta Luis con `~/.venvs/sat-xls`. Es una herramienta de desarrollo, como los scripts de verificación; no es código del producto.
- Recibe como argumento la carpeta `Docs/`, donde están los Excel. Nunca los copia al repositorio.
- Antes de entregar cada versión de la herramienta, Claude la prueba contra los cuatro `.xls`, que Luis sube a la sesión: son públicos.

### 3. Definición declarativa por catálogo

Un archivo de definiciones en `tools/sat-catalogs/` describe cada catálogo:

| Elemento | Contenido |
| --- | --- |
| Origen | Archivo y **nombre exacto de cada hoja**, con sus espacios y mayúsculas; varias hojas cuando el catálogo viene partido |
| Metadatos | Fila donde están la versión, la revisión y la fecha de publicación del catálogo |
| Encabezado | Una o dos filas de encabezado y la fila donde empiezan los datos |
| Columnas | Para cada columna: su **posición**, el texto esperado en cada fila del encabezado, la columna destino de la tabla (ADR-06) y su normalización |
| Clave | Su formato, para restituir los ceros a la izquierda |
| Renglones descartados | Reglas para los renglones que no son datos |
| Anomalías declaradas | Las excepciones revisadas una vez por Luis y Claude, cada una con su motivo |

Normalizaciones disponibles:

| Normalización | Resultado |
| --- | --- |
| Texto | Sin espacios al inicio ni al final |
| Clave | Texto con el formato de la definición (por ejemplo, dos dígitos con ceros a la izquierda) |
| Sí o no | Verdadero o falso; cualquier otro valor es un error |
| Fecha | `AAAA-MM-DD`; vacío o un valor que no es fecha ("Por definir") queda nulo |
| Decimal | Texto con el valor exacto, sin pasar por números de punto flotante |
| Lista | Una lista de claves, para generar una tabla hija (los regímenes del receptor de `c_UsoCFDI`) |

Los datos se guardan tal como vienen cuando su relación con otro catálogo es trabajo de la validación: por ejemplo, el impuesto de `c_TasaOCuota` se guarda por nombre (E07).

### 4. Verificaciones de la herramienta

Si alguna verificación falla, la herramienta no genera ningún archivo.

| Verificación | Comportamiento |
| --- | --- |
| Hojas | Cada hoja de la definición existe con su nombre exacto |
| Encabezados | Cada columna coincide con su texto esperado en cada fila del encabezado. Así un cambio de formato del SAT se detecta antes de cargar |
| Claves únicas | No hay claves repetidas, considerando la llave compuesta donde aplica (ADR-06) |
| Claves contra el XSD | Si el catálogo tiene enumeración en un XSD local, cada clave del Excel existe en ella. Las claves del XSD que no vienen en el Excel se reportan, sin fallar |
| Valores | Cada valor cumple su normalización |
| Anomalías | Cualquier anomalía no declarada falla. La clave vacía de `c_TipoDeComprobante` se revisa una vez y queda declarada con su motivo |
| Huérfanos | Una colonia sin código postal, o un municipio o localidad sin estado, se **reportan** sin fallar: no hay llaves foráneas entre catálogos (ADR-06) |

La herramienta escribe un reporte en `tools/sat-catalogs/ultimo-reporte.md` con los conteos por catálogo, las claves del XSD ausentes del Excel, las anomalías declaradas que aplicó y los huérfanos. El reporte se guarda en el repositorio con cada actualización.

### 5. Formato de los datos versionados

- Un archivo por catálogo, y uno por cada tabla hija, en `src/DanmaobCfdi.Catalogs/SatData/`, con el nombre de su tabla y la extensión `.jsonl` (por ejemplo `c_Colonia.jsonl`).
- **JSON por líneas**, en UTF-8 sin BOM y con fin de línea LF:
  - la primera línea es un arreglo con los nombres de las columnas de la tabla;
  - cada línea siguiente es un arreglo con los valores de un renglón, en el mismo orden;
  - los valores son texto, verdadero o falso, o nulo. Las fechas y los decimales van como texto.
  - Los renglones van ordenados por su llave, para que la diferencia de Git muestre exactamente qué cambió el SAT.
- `SatData/manifest.json` registra, por catálogo:
  - el Excel de origen y su hash SHA-256, o "datos fijos";
  - la versión, revisión y fecha de publicación del SAT, con nulo cuando no existen;
  - el número de registros;
  - el hash SHA-256 de su archivo de datos.
- Los archivos de `SatData/` se incrustan en el ensamblado de `DanmaobCfdi.Catalogs`, con su ruta relativa como nombre lógico (igual que el ADR-03).
- `src/.gitattributes` marca `DanmaobCfdi.Catalogs/SatData/**` como `-text`: Git no altera sus bytes, pero sigue mostrando sus diferencias.
- El ensamblado de `DanmaobCfdi.Catalogs` crece unos 35 MB, sobre todo por colonias, códigos postales y claves de productos.

### 6. Aplicación por migración

- Cada actualización de datos es una **migración de datos** sin cambios de modelo. Se genera vacía con `dotnet ef migrations add`, y su cuerpo llama a un ayudante con la lista de catálogos que actualiza.
- Para cada catálogo de la lista, el ayudante:
  1. lee su archivo incrustado y comprueba su hash contra el manifiesto;
  2. borra todos sus renglones, y antes los de sus tablas hijas;
  3. inserta los renglones nuevos en lotes de 1,000 por instrucción, con los valores escritos como literales SQL escapados;
  4. reemplaza su renglón en `sat.CatalogoVersion` con los datos del manifiesto, la versión del componente (ADR-02, sección 2) y la fecha de aplicación en UTC.
- Todo ocurre dentro de la transacción de la migración: si algo falla, el catálogo queda como estaba.
- **El ayudante siempre carga el archivo incrustado actual.** Una migración de datos antigua sigue funcionando en una base nueva aunque el archivo haya cambiado: la última migración deja los datos vigentes. Por eso solo se conservan los datos actuales, no cada publicación anterior.
- El ayudante y la lectura de los archivos los construye DEV JR. Sus pruebas usan archivos pequeños, sin base de datos (ADR-04, sección 7). La carga real la verifica Luis con Docker y `sqlcmd`.

### 7. Datos fijos

- `c_MotivoCancelacion` y `c_TipoCadenaPago` se escriben a mano en el archivo de definiciones, con el Anexo 20 como fuente citada, y producen el mismo formato de datos y la misma entrada en el manifiesto.
- Luis valida los valores contra el Anexo 20 antes de la primera carga (D-19 y D-25).
- Cambiar uno de estos valores es una versión nueva del componente, igual que cualquier otro dato de catálogo.

### 8. Actualización por publicación del SAT

1. Luis descarga el Excel nuevo desde el navegador y lo verifica con `file`.
2. Luis ejecuta la herramienta. Si un encabezado o una hoja cambiaron, falla, y Claude actualiza la definición.
3. Luis y Claude revisan el reporte y la diferencia de Git de `SatData/`.
4. DEV JR agrega la migración de datos con los catálogos que cambiaron.
5. Luis la aplica con Docker y la verifica con `sqlcmd`.
6. Sale una versión PARCHE del componente, o MENOR si cambió la estructura de una tabla (ADR-02, sección 1).

## Fuera del alcance de este ADR

- La estructura exacta de cada tabla: TS-03-2 y TS-03-3.
- La definición concreta de cada catálogo: los elementos de E04.
- La relación entre el impuesto por nombre de `c_TasaOCuota` y `c_Impuesto`: E07.
- Los catálogos de los complementos de Retenciones: TK-12-1.

## Alternativas consideradas

| Alternativa | Por qué no se eligió |
| --- | --- |
| Leer los Excel en cada producto al instalar o actualizar | Cada producto llevaría los Excel y una biblioteca que lea `.xls`, y los datos dejarían de viajar con la versión del componente (D-16) |
| Biblioteca .NET que lea `.xls` en una consola de la solución | DEV JR tendría que construir un lector complejo sin poder abrir los Excel para verificarlo |
| Convertir los `.xls` a `.xlsx` antes de leerlos | Un paso manual más, con riesgo de que la conversión altere fechas o claves |
| Leer columnas por el texto del encabezado | Falla con encabezados repetidos y de dos filas |
| Detener la carga por un huérfano | Los datos del SAT no son consistentes entre catálogos; la carga completa fallaría por un renglón |
| CSV o TSV | Las descripciones contienen comas y comillas; el formato necesitaría un analizador propio. Los tabuladores chocan con las normalizaciones de los scripts |
| Archivos comprimidos | Ahorran espacio, pero se pierde la diferencia legible de cada publicación del SAT |
| Una instrucción `INSERT` en el código de la migración por cada renglón | Archivos de migración de decenas de MB, imposibles de revisar y de compilar con holgura |
| Carga masiva fuera de las migraciones | Contradice el ADR-02: los datos llegan con la versión y se aplican con `dotnet ef database update` |
| Conservar los datos de cada publicación anterior | El ensamblado crecería con cada publicación del SAT sin ningún uso |

## Consecuencias

**Lo que se facilita:**

- Un cambio de formato del SAT se detecta en la herramienta, no en la base de datos.
- La diferencia de Git de `SatData/` muestra exactamente qué cambió el SAT en cada publicación.
- Los productos aplican catálogos igual que cualquier migración, sin manejar Excel.
- Una base nueva queda con los datos vigentes aunque se apliquen todas las migraciones desde el inicio.

**Lo que exige cuidado:**

- **Tiempo de aplicación:** la primera carga inserta unos 400 mil renglones y puede tardar varios minutos.
- **Tamaño:** el ensamblado de `DanmaobCfdi.Catalogs` pesa unos 35 MB más.
- **Datos fijos:** sus valores dependen de la validación de Luis contra el Anexo 20.
- **Herramienta y datos juntos:** la herramienta y su definición se actualizan junto con los datos; un archivo de `SatData/` editado a mano no coincide con su hash del manifiesto y la migración falla.

**Trabajo desbloqueado:**

| Elemento | Qué toma de este ADR |
| --- | --- |
| Herramienta y definiciones de E04 | Secciones 2, 3, 4 y 7 |
| Formato y manifiesto | Sección 5 |
| TS-04-3 | Migración de datos y registro de la versión cargada (sección 6) |
| Actualizaciones futuras | Sección 8 |
