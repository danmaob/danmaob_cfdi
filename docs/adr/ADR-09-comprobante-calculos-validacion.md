# ADR-09 — Modelo del comprobante, cálculos fiscales y validación previa al timbrado

**Estado:** Aceptado
**Fecha:** 21 de octubre de 2026
**Historia:** SP-05-1 (épica E05 — Modelo del comprobante y cálculos fiscales)
**Requerimientos:** P08; §3.2; §3.4; §4; §8; TK-05-1
**Depende de:** ADR-01 (estructura de la solución), ADR-03 (recursos oficiales del SAT), ADR-04 (estrategia de pruebas), ADR-06 (catálogos: modelo y consulta) y ADR-08 (sellado y manejo del CSD)

## Contexto

Los modelos de CFDIAPI40 (unas 9 mil líneas en `Models/CFDI`) describen CFDI 4.0 con Ingreso, Egreso, Traslado y Pagos, `InformacionGlobal`, `CfdiRelacionados` y `Addenda`. La revisión del código mostró estos defectos (informe de revisión, sección 4.4):

- **Cálculos dentro de los modelos.** Los totales se calculaban en los getters: por ejemplo, `Descuento` sumaba los conceptos cada vez que se leía.
- **Redondeo equivocado.** `Math.Round(x, 2)` sin modo redondea al par y siempre a 2 decimales, sin importar la moneda.
- **Cuatro clases casi iguales.** Una base partida en dos archivos y cuatro derivadas (Ingreso, Egreso, Traslado y Pago), cada una serializando sus complementos a mano con `XmlSerializer`.
- **Responsabilidades mezcladas.** Validación contra la base de datos, la URL del código QR, `DataAnnotations` y atributos de JSON propios de una API web.

TK-05-1 quedó cerrada el 21 de octubre de 2026. Luis confirmó quince reglas tomadas de la Guía de llenado del CFDI 4.0 (16/01/2026), del Estándar del Anexo 20 (DOF 13/01/2022) y de la Guía de llenado del complemento de pagos (revisión del 29/12/2025). También decidió el desempate del redondeo, que esos documentos no definen: **alejarse del cero**.

| Regla | Contenido | Fuente |
| --- | --- | --- |
| 1 | Los decimales de cada moneda vienen de `c_Moneda` (de cero a cuatro) | Estándar, pág. 71; Guía, pág. 10 |
| 2 a 4 | En los conceptos, de cero a seis decimales; se redondea al final del cálculo, nunca antes, y con el mismo número de decimales en todos los importes | Guía, pág. 24 a 30 y preguntas 37 a 39 |
| 5 a 7 | `SubTotal`, `Descuento` y `Total` con los decimales de la moneda; `Total` incluye impuestos locales | Estándar, pág. 71 y 72 |
| 8 a 10 | Límites del `Importe` del concepto y de los impuestos del concepto: el inferior truncado, el superior redondeado hacia arriba | Estándar, pág. 76 a 78; Guía, pregunta 41 |
| 11 y 12 | Resumen de impuestos agrupado por `Impuesto`, `TipoFactor` y `TasaOCuota`; `TasaOCuota` con seis decimales | Estándar, pág. 80 y 81; Guía, pág. 39 a 41 y pregunta 40 |
| 13 a 15 | Pagos: `EquivalenciaDR` con hasta 10 decimales, margen por redondeo con 10^−16 y totales en MXN | Guía de pagos, pág. 22 a 34 y 41 |

## Decisión

### 1. Frontera entre el componente y el producto

El producto entrega **solo datos de negocio**. El componente calcula todo lo que se deriva de ellos.

| Lo entrega el producto | Lo calcula el componente |
| --- | --- |
| Emisor y receptor | `Importe` de cada concepto |
| Moneda, tipo de cambio y fecha con zona horaria (`DateTimeOffset`, ADR-08) | `Base` e importe de cada impuesto del concepto |
| Serie, folio, forma y método de pago, exportación, lugar de expedición | Resumen de impuestos |
| Conceptos: `Cantidad`, `ValorUnitario`, `Descuento` y sus datos descriptivos | `SubTotal`, `Descuento` y `Total` |
| Impuestos de cada concepto: `Impuesto`, `TipoFactor` y `TasaOCuota` | Número de certificado, certificado y sello (ADR-08) |
| CFDI relacionados y datos de los complementos | Totales de los complementos que los tengan |
| El CSD de la operación (ADR-08) | |

El modelo de entrada **no tiene campos para importes calculados**. El producto no puede enviar un total que contradiga el cálculo del componente.

### 2. Dos modelos separados

| Modelo | Visibilidad | Contenido |
| --- | --- | --- |
| De entrada | público | Los datos de la sección 1 para cada comprobante y complemento. Es el contrato con el producto |
| XML | `internal` | La forma exacta del XSD vigente, sin lógica: solo propiedades y atributos de serialización |

- Los modelos de CFDIAPI40 se reutilizan **solo como forma del modelo XML**. Se les quitan los getters con cálculos, `DataAnnotations`, los atributos de JSON, la URL del QR y el acceso a la base de datos.
- Sus atributos se ajustan al XSD vigente (TS-05-3): `Año` desde 2019, longitudes de 300 caracteres y `EquivalenciaDR` con 10 decimales.
- Carta Porte 3.1 se construye desde su XSD, no desde las clases de Carta Porte 2.0.
- La lectura de XML de terceros tiene su propio modelo (ADR-05). No comparte el modelo XML de emisión.

### 3. Una sola clase de comprobante y un namespace por versión

- Hay **una sola clase `Comprobante` de CFDI 4.0**, con `TipoDeComprobante` como dato. Desaparecen las cuatro clases derivadas.
- Las reglas propias de cada tipo, como "Traslado y Pago llevan `SubTotal` y `Total` en cero", viven en el cálculo y en la validación, no en herencia.
- Cada versión del SAT vive en su propio namespace dentro de `DanmaobCfdi.Core`: `Cfdi40`, `Pagos20`, `CartaPorte31`, `ImpLocal10`, `LeyendasFisc10` y `Retenciones20` (ADR-01, sección 8). Una versión nueva se agrega en un namespace nuevo sin modificar las demás.

### 4. Serialización

- `XmlSerializer` sobre las clases del modelo XML, con un serializador en caché por tipo, creado una sola vez.
- **Complementos de forma genérica.** Cada complemento declara su namespace, su prefijo y su `schemaLocation`. Un solo serializador los convierte en elementos y los agrega al nodo `Complemento`, junto con sus namespaces y el `xsi:schemaLocation` del comprobante. No se escribe código de serialización por complemento.
- **Todos los importes son `decimal`, nunca `double`.** El número de decimales que aparece en el XML es la escala del `decimal` que deja el cálculo. Por eso el cálculo es el único lugar que fija los decimales.
- El XML se escribe en UTF-8 sin BOM.

### 5. Servicio de cálculos

Vive en `DanmaobCfdi.Core.Cfdi40`, sin estado ni base de datos. Aplica las reglas 1 a 15, en `decimal` y con desempate alejándose del cero (`MidpointRounding.AwayFromZero`) en todo redondeo.

| Nivel | Regla |
| --- | --- |
| Concepto | `Importe` = `Cantidad` × `ValorUnitario`. `Base` de cada impuesto = `Importe` − `Descuento`. Importe del impuesto = `Base` × `TasaOCuota`. Cada uno se calcula con toda la precisión del `decimal` y se redondea a 6 decimales. **Nunca** se redondea a los decimales de la moneda en el concepto |
| Comprobante | `SubTotal` y `Descuento` = redondeo de la suma de los conceptos a los decimales de la moneda |
| Resumen de impuestos | Agrupado por `Impuesto` (retenciones) o por `Impuesto`, `TipoFactor` y `TasaOCuota` (traslados); cada `Base` e `Importe` = redondeo de la suma a los decimales de la moneda; los totales del resumen son la suma de sus hijos |
| Total | `SubTotal` − `Descuento` + traslados − retenciones + traslados locales − retenciones locales. Todos los sumandos ya tienen los decimales de la moneda |
| Traslado y Pago | `SubTotal` y `Total` en cero, sin resumen de impuestos |
| Pagos 2.0 | Totales en MXN y margen de `EquivalenciaDR` según las reglas 13 a 15; su detalle lo fija la historia de Pagos con el Estándar del complemento de pagos |

`decimal.Round` nunca aumenta la escala: `1000.00` sigue escribiéndose como `1000.00`, y no como `1000.000000`.

### 6. Decimales de la moneda

- El servicio de cálculos **recibe los decimales de la moneda como parámetro**. No consulta catálogos.
- El servicio que orquesta la emisión los obtiene de `c_Moneda` con la interfaz de consulta de catálogos (ADR-06, sección 7). Así, `Core` sigue sin base de datos, y las pruebas pasan los decimales directo.
- **Precisión al ADR-06.** `c_Moneda` pasa a la sección 5 del ADR-06, con datos propios: `Decimales` como entero y `PorcentajeVariacion` como `decimal`, más una consulta específica que los devuelve. Con la regla general de esa sección se habrían guardado como texto.

### 7. Capas de validación

La preparación de un comprobante sigue este orden:

| Capa | Qué comprueba |
| --- | --- |
| 1. Entrada | Datos completos, longitudes, rangos y cantidades positivas |
| 2. Cálculo | Que el cálculo sea posible: por ejemplo, un `Descuento` que no supere el `Importe` |
| 3. Armado | El XML a partir del modelo |
| 4. Estructura | El XML contra los esquemas compilados de `SatCompiledResources` (ADR-03) |
| 5. Catálogos | Claves vigentes a la fecha del comprobante y relaciones entre catálogos (ADR-06). No se bloquea por `c_NumPedimentoAduana` ni `c_PatenteAduanal` |
| 6. Reglas del Anexo 20 | Las que elija SP-07-1, y las de la guía de Carta Porte |
| 7. Sello | Validaciones del CSD y sello (ADR-08) |

- Cada capa reporta **todos** sus errores juntos.
- Si una capa falla, las siguientes no se ejecutan. Por ejemplo, un XML que no pasa el XSD no se valida contra catálogos.

### 8. Forma del error de validación

El resultado es explícito, como en el ADR-08: un éxito o una lista de errores. Cada error tiene:

| Campo | Contenido |
| --- | --- |
| Código | Código propio del componente, estable entre versiones |
| Código del SAT | El código de la matriz de errores cuando la regla corresponde a uno, por ejemplo `CFDI40108`; vacío si no |
| Capa | La de la sección 7 |
| Ruta | La posición del campo, por ejemplo `Conceptos[2]/Impuestos/Traslados[1]/Importe` |

- El error **no lleva valores del comprobante** (§8). El producto conoce sus datos y los muestra si lo necesita.
- Los nombres exactos los fija TS-07-1.

### 9. Operación pública: preparar un CFDI 4.0

- Una operación pública, **preparar**, recibe el modelo de entrada y el CSD. Ejecuta las capas 1 a 7 y devuelve el XML sellado, listo para el PAC, o la lista de errores.
- El envío al PAC y su resultado (éxito, rechazo o error técnico) se deciden en SP-09-1.
- La operación es asíncrona y acepta cancelación, porque la capa 5 consulta la base de datos.

### 10. Pruebas que fija este ADR para E05 y E06

| Tema | Pruebas |
| --- | --- |
| Redondeo | Casos de 5 exacto en el desempate (por ejemplo `0.125 → 0.13` y `0.005 → 0.01`); una moneda con 0 y otra con 3 decimales |
| Ejemplos oficiales | Pregunta 37 de la Guía: impuestos que suman `0.7104` dan `0.71`. Pregunta 41: `924.224956` tiene límites `924.22` y `924.23` |
| Límites | Cada `Importe` y cada impuesto calculados caen dentro de los límites de las fórmulas del Estándar, calculados en la prueba de forma independiente |
| Totales | `SubTotal`, `Descuento`, resumen y `Total` con varias tasas, exentos y retenciones |
| Extremo a extremo | El XML generado contra referencias producidas por el generador de TS-01-6, validadas con `xmllint` y con cadena de `xsltproc` |

## Fuera del alcance de este ADR

| Tema | Estado |
| --- | --- |
| Addenda | CFDIAPI40 la tenía; ningún consumidor la ha pedido. Pendiente de decisión de Luis, sin construirla |
| Representación impresa (PDF) y URL del código QR | Pendientes de decisión de Luis |
| Factura global (`InformacionGlobal`) | Con la versión `CFDI_ERP` |
| Retenciones 2.0 | Sigue este mismo patrón; su detalle espera a TK-12-1 |
| Envío al PAC | SP-09-1 |

## Alternativas consideradas

| Alternativa | Por qué se descarta |
| --- | --- |
| Conservar los cálculos en los getters de los modelos | Mezcla forma y reglas, impide probar los cálculos por separado y repite el cálculo en cada lectura |
| Que el producto envíe los importes ya calculados | Cada producto repetiría las reglas del SAT, y un error de redondeo sería un rechazo del PAC |
| Un solo modelo público con la forma del XML | Expone campos calculados al producto y lo obliga a conocer el detalle del XSD |
| Conservar las cuatro clases derivadas | Duplican atributos y serialización; las diferencias entre tipos son reglas, no estructura |
| Clases generadas con `xsd.exe` | Ya existían en CFDIAPI40 y nunca compilaban; son difíciles de leer y de mantener |
| Construir el XML a mano con `XDocument` | Más código y más errores de namespaces que `XmlSerializer` con atributos |
| Redondear los conceptos a los decimales de la moneda | Contradice la regla de redondear al final del cálculo (reglas 3 y 4) |
| Redondeo al par (el de .NET por omisión) | Luis decidió alejarse del cero, el redondeo aritmético que menciona la Guía |
| `double` para los importes | Pierde precisión decimal y no conserva la escala |
| Que el cálculo consulte `c_Moneda` | Metería la base de datos en `Core` (ADR-01) |

## Consecuencias

**Se facilita:**

- El producto solo describe la operación; los importes del SAT siempre salen del mismo cálculo probado.
- Los cálculos se prueban sin XML, sin base de datos y sin red.
- Una versión nueva de un complemento se agrega en su propio namespace, sin tocar las demás.
- Un error de validación le dice al producto qué campo falla, con el código del SAT cuando existe.

**Exige cuidado:**

- El cálculo es el único lugar que fija la escala de los `decimal`. Ninguna otra parte del componente puede redondear importes.
- TS-03-2 o TS-03-3 deben crear `c_Moneda` con `Decimales` y `PorcentajeVariacion` tipados (sección 6), y el ADR-06 registra la precisión en su historial.
- Las reglas de Pagos 2.0 que la Guía remite a su Estándar se completan cuando se descargue ese documento, en la historia de Pagos.

**Desbloquea:** TS-05-1 a TS-05-5, TS-06-1 a TS-06-3, SP-07-1 y TS-07-1 a TS-07-6, y las historias de E11 y E12 que emiten con este patrón.
