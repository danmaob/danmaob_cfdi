# ADR-04 — Estrategia de pruebas

**Estado:** Aceptado
**Fecha:** 19 de octubre de 2026
**Historia:** SP-01-4 (épica E01 — Fundamentos del componente)
**Requerimientos:** P04; §5.1; §8; §10.4
**Depende de:** ADR-01 (estructura de la solución) y ADR-03 (recursos oficiales del SAT)

## Contexto

CFDIAPI40 no tenía ninguna prueba. Cada cambio se verificaba timbrando, y DANMAOB paga los ajustes por cambios del SAT, así que sin pruebas automáticas cada actualización cuesta más y arriesga más.

Las instrucciones fijan tres condiciones:

- Las pruebas nunca se conectan a la base de datos ni a un PAC productivo. Las pruebas contra el ambiente de pruebas del PAC las ejecuta solo Luis (sección 5.1).
- El comportamiento no trivial (sellado, transformación XSLT, lectura de XML y carga de catálogos) exige una prueba de ejecución real lo antes posible, no solo que compile (sección 10.4).
- El CSD y su contraseña nunca van en código ni en configuración versionada (sección 8).

Además, el autorreporte de DEV JR nunca es evidencia. La aceptación de cada etapa sale del script de verificación, que cuenta las pruebas y sus nombres en los resultados de `dotnet test`.

Evidencia del 19 de octubre de 2026:

| Hecho | Detalle |
| --- | --- |
| Herramientas en la Mac de Luis | `xsltproc` (libxml 2.9.13, libxslt 1.1.35), `xmllint` (libxml 2.9.13) y OpenSSL 3.6.3 |
| Paquetes de pruebas del ERP | `xunit` 2.9.3, `xunit.runner.visualstudio` 3.1.4, `Microsoft.NET.Test.Sdk` 17.14.1, `NetArchTest.Rules` 1.3.2 y `coverlet.collector` 6.0.4 |
| CSD de pruebas | El SAT publica en su portal un paquete de CSD de pruebas con sus archivos `.cer` y `.key`, la contraseña y los datos fiscales de cada RFC de prueba. Luis lo descargó (`Certificados_de_Prueba.zip`) |

## Decisión

### 1. Qué corre en `dotnet test`

Todas las pruebas de `tests/DanmaobCfdi.Tests` corren **sin red y sin base de datos**. Hay tres tipos:

| Tipo | Qué prueba | Cómo |
| --- | --- | --- |
| Unitarias | Lógica pura: cálculos, reglas, construcción de resultados | Con dobles de prueba para las dependencias |
| De ejecución real con recursos | Transformación XSLT, validación contra XSD, sellado, lectura de XML y extracción de catálogos | Ejecución real con los recursos incrustados del SAT y archivos de referencia, sin dobles para lo que se prueba |
| De arquitectura | Las reglas de dependencias del ADR-01 | NetArchTest.Rules |

Las pruebas contra el ambiente de pruebas de PAX **no** viven en este proyecto y nunca corren con un `dotnet test` normal. Dónde viven y cómo reciben las credenciales lo decide SP-09-1; solo Luis las ejecuta.

Una función fiscal no está terminada hasta probarse en el ambiente de pruebas del PAC (instrucciones, sección 6). Esa prueba es un paso de Luis, fuera de `dotnet test`.

### 2. Resultados esperados de una fuente independiente

Un valor esperado nunca lo calcula el propio componente: una prueba así solo demostraría que el componente coincide consigo mismo. Los valores esperados salen de herramientas ajenas a .NET:

| Valor esperado | Herramienta | Forma |
| --- | --- | --- |
| Cadena original | `xsltproc --nonet` (libxslt) | Con un catálogo XML que convierte cada dirección `http://www.sat.gob.mx/` en el archivo correspondiente de `src/DanmaobCfdi.Core/SatResources/`, los mismos archivos que usa el componente |
| Sello | OpenSSL | Firma SHA-256 con RSA de la cadena esperada, con la llave del CSD de pruebas, codificada en base 64. Este algoritmo siempre produce el mismo sello para la misma cadena y la misma llave |
| Validez estructural de un XML de referencia | `xmllint --nonet --schema` | Con el mismo catálogo XML |

- Claude escribe los scripts que generan estos valores y Luis los ejecuta. Los resultados se guardan junto al XML de referencia, tal como los produce la herramienta, byte por byte, sin agregar un salto de línea final.
- Si el componente y la herramienta no coinciden, se investiga la causa. Nunca se ajusta un valor esperado para que coincida con el componente.
- Que .NET y libxslt produzcan la misma cadena original con los mismos archivos del SAT es además la prueba real de que las hojas no exigen XSLT 2.0 (ADR-03).

### 3. Archivos de referencia

Viven en `tests/DanmaobCfdi.Tests/Fixtures/`:

| Carpeta | Contenido |
| --- | --- |
| `Cfdi40/` | CFDI 4.0, con o sin impuestos locales y leyendas fiscales |
| `Pagos20/` | CFDI con complemento de pagos 2.0 |
| `CartaPorte31/` | CFDI con complemento Carta Porte 3.1 |
| `Retenciones20/` | Comprobantes de Retenciones 2.0 |
| `Cancelacion/` | Acuses de cancelación |
| `NoSoportados/` | Versiones que la lectura debe reportar como no soportadas (por ejemplo CFDI 3.3 o Carta Porte 3.0) |
| `Invalidos/` | XML mal formado, con DTD o con entidades externas, y XML que no pasa el XSD |
| `Csd/` | CSD públicos de pruebas del SAT (sección 5) |

- Cada caso usa un mismo nombre base: `<caso>.xml`, `<caso>.cadena.txt` y `<caso>.sello.txt`, según lo que aplique.
- Se copian a la carpeta de salida de las pruebas; no se incrustan.
- `tests/.gitattributes` marca `DanmaobCfdi.Tests/Fixtures/**` como `-text`, para que Git nunca altere sus bytes.
- Ningún script de corrección mecánica ni de normalización de tabuladores toca `Fixtures/`.

### 4. Origen de los XML de referencia

En este orden:

1. **Sintéticos.** Los redacta Claude como datos de prueba, no como código del producto. Usan los RFC de los CSD de pruebas con los datos fiscales que publica el SAT para cada uno (nombre, código postal, régimen), y datos ficticios para lo demás. Llegan en los paquetes, igual que los ADR, y Luis los copia con su script, que antes los valida contra los XSD con `xmllint`. Los casos negativos de la lectura también son sintéticos. Un Timbre Fiscal Digital sintético sirve para las pruebas de lectura, porque la lectura no verifica sellos (D-30).
2. **Timbrados en el ambiente de pruebas de PAX.** Cuando exista el contrato, se suman como referencia real, con un Timbre Fiscal Digital auténtico y RFC de pruebas.
3. **Nunca XML de clientes**, ni siquiera anonimizados.

DEV JR nunca crea ni edita archivos de `Fixtures/`; los prompts solo le dan su ruta.

### 5. CSD de pruebas

- Viven en `tests/DanmaobCfdi.Tests/Fixtures/Csd/`.
- Solo entran CSD públicos de pruebas del SAT: uno de persona moral y uno de persona física.
- Nombres: `<RFC>.cer`, `<RFC>.key` y `<RFC>.password.txt`.
- `.gitignore` recibe una excepción solo para los `.cer` y `.key` de esa carpeta. En cualquier otra ruta siguen excluidos.
- El script de copia de Luis comprueba que cada archivo sea idéntico al del paquete descargado del SAT y que el RFC del sujeto del certificado coincida con el nombre del archivo.
- **Excepción documentada a la regla de secretos:** la contraseña de los CSD de pruebas la publica el SAT junto con ellos, así que se guarda en `<RFC>.password.txt`. Ninguna otra contraseña ni ningún otro CSD entran al repositorio, a las pruebas ni al chat.

### 6. Fechas y vigencia

- Ninguna prueba depende del reloj del sistema. El componente nunca usa `DateTime.Now`, `DateTime.UtcNow`, `DateTimeOffset.Now` ni `DateTimeOffset.UtcNow`; recibe la fecha actual por `TimeProvider`.
- Las pruebas usan fechas fijas, con un doble de `TimeProvider` escrito a mano cuando haga falta.
- La vigencia del CSD se evalúa contra la fecha del comprobante. Por eso, cuando el CSD de pruebas caduque, las pruebas con fechas fijas dentro de su vigencia siguen pasando.

### 7. Catálogos sin base de datos

| Qué | Cómo se prueba |
| --- | --- |
| Lo que consulta catálogos (validación y descripciones) | Con un doble escrito a mano de la interfaz de acceso a catálogos. No se usan EF Core InMemory ni SQLite, que se comportan distinto que SQL Server |
| El proceso de carga desde los archivos del SAT | Con extractos pequeños (unas filas por catálogo) generados por un script de Luis, nunca con los Excel completos. DEV JR no abre los Excel. El detalle es de SP-04-1 |
| Migraciones y carga real en SQL Server | No es prueba automática: es un paso de Luis con Docker y `sqlcmd`, con consultas de solo lectura y pruebas de reglas de la base dentro de transacciones que se deshacen |

### 8. Convenciones

- **Paquetes:** las mismas versiones que el ERP como punto de partida: `xunit` 2.9.3, `xunit.runner.visualstudio` 3.1.4, `Microsoft.NET.Test.Sdk` 17.14.1 y `NetArchTest.Rules` 1.3.2. TS-01-1 las fija en el `.csproj`. No se usa `coverlet.collector`, porque no hay meta de cobertura.
- **Dobles de prueba:** escritos a mano. No se usan bibliotecas de simulación.
- **Organización:** una clase de prueba por clase probada, en la carpeta que replica su ruta (ADR-01, sección 8).
- **Nombres:** describen el comportamiento con la forma `Metodo_Condicion_Resultado`.
- **Total de pruebas:** cada etapa fija el total esperado y los nombres de las pruebas nuevas, y el script de verificación los comprueba en los resultados de `dotnet test`.
- **Sin meta de cobertura:** un porcentaje no dice si se probó lo que importa. Lo que importa queda fijado por la sección 10.4 de las instrucciones y por los criterios de aceptación de cada elemento.

## Fuera del alcance de este ADR

- Dónde viven y cómo reciben credenciales las pruebas contra el ambiente de pruebas de PAX: SP-09-1.
- Cómo se generan los extractos de catálogos y qué formato tienen: SP-04-1.
- El esquema con que se valida el acuse de cancelación: SP-02-1.

## Alternativas consideradas

| Alternativa | Por qué no se eligió |
| --- | --- |
| Valores esperados calculados con el propio componente | La prueba solo demostraría que el componente coincide consigo mismo |
| Validar solo timbrando en el ambiente de pruebas de PAX | Lento, depende del contrato y de la red, y no se puede ejecutar en cada etapa |
| EF Core InMemory o SQLite para probar catálogos | Se comportan distinto que SQL Server en llaves, índices, transacciones y comparación de texto |
| Pruebas contra SQL Server en Docker dentro de `dotnet test` | Las instrucciones lo prohíben, y Docker y DEV JR no pueden correr al mismo tiempo por memoria |
| Bibliotecas de simulación (Moq, NSubstitute) | Agregan una dependencia y un estilo que DEV JR maneja con menos precisión que un doble escrito a mano |
| xUnit 3 | Se aparta del ERP; las versiones del ERP ya están probadas con DEV JR y con los scripts de verificación |
| Meta de porcentaje de cobertura | No dice si se probó lo que importa |
| CSD de una empresa real | Prohibido (sección 8): cualquier cosa sellada llevaría una firma auténtica, y su llave y contraseña quedarían expuestas |
| XML de clientes anonimizados | Riesgo de exponer datos fiscales reales; los sintéticos y los timbrados en pruebas bastan |

## Consecuencias

**Lo que se facilita:**

- Cada etapa con comportamiento real se acepta con una prueba de ejecución real, no con el reporte de DEV JR.
- Las cadenas originales y los sellos esperados tienen una fuente independiente de .NET.
- Las pruebas no dependen del reloj, de la red, de la base de datos ni de la vigencia del CSD de pruebas.
- Los paquetes de prompts reutilizan las versiones y las verificaciones del ERP.

**Lo que exige cuidado:**

- Los XML sintéticos deben coincidir con los datos fiscales publicados para cada RFC de prueba. Si no, PAX los rechazará cuando se prueben en su ambiente.
- Los archivos de `Fixtures/` se mantienen byte por byte: nadie los normaliza ni los reformatea.
- Cuando el SAT publique nuevos CSD de pruebas, se reemplazan con el mismo script y se regeneran los sellos esperados.

**Pruebas de ejecución real que exige cada elemento (sección 10.4):**

| Elemento | Prueba real |
| --- | --- |
| TS-01-5 | Compilación de la XSLT y de los conjuntos de esquemas con los recursos incrustados |
| TS-01-6 | Cadena original de un XML de referencia contra la generada por `xsltproc` |
| Elementos de lectura (E02) | Lectura de los XML de referencia válidos, no soportados e inválidos |
| Elementos de sellado (E08) | Sello de una cadena de referencia contra el generado por OpenSSL |
| Elementos de carga (E04) | Extracción de los extractos de catálogos |

**Trabajo desbloqueado:**

- TS-01-6: proyecto de pruebas con archivos de referencia y prueba real de cadena original.
- SP-02-1 y SP-08-1, que parten de esta estrategia para la lectura y el sellado.
- TS-01-1, que fija las versiones de los paquetes de pruebas.
