# ADR-05 — Lectura y validación estructural de XML de terceros

**Estado:** Aceptado
**Fecha:** 19 de octubre de 2026
**Historia:** SP-02-1 (épica E02 — Lectura de XML de terceros)
**Requerimientos:** P07; D-28; D-29; D-30; D-31; D-38; §3.4; §4; §8
**Depende de:** ADR-01 (estructura de la solución), ADR-03 (recursos oficiales del SAT) y ADR-04 (estrategia de pruebas)

## Contexto

La lectura de XML de terceros es lo primero que necesita DANMAOB_TMS (entregable 2). El cliente sube los CFDI que emitió con su sistema actual, y el TMS los usa para alimentar su cuenta por cobrar y para completar sus catálogos de unidades y operadores. CFDIAPI40 no tenía nada parecido, salvo una extracción mínima del Timbre Fiscal Digital.

El alcance ya está decidido:

| Decisión | Contenido |
| --- | --- |
| D-28 | Solo CFDI 4.0 con Carta Porte 3.1, Pagos 2.0 y Timbre Fiscal Digital 1.1. Cualquier otra versión se reporta como no soportada, sin fallar |
| D-29 | La evidencia de cancelación es el acuse de cancelación en XML |
| D-30 | Validación solo estructural, contra los XSD locales. No se verifican los sellos del emisor ni del SAT |
| D-31 | La lectura es transitoria: se usa mientras el producto no tenga su propia facturación |
| D-38 | La lectura no depende de los catálogos. En `CFDI_Lectura` entrega claves; las descripciones llegan después de la carga de catálogos |

El XML viene de sistemas ajenos y se trata como contenido no confiable: DTD y entidades externas deshabilitadas, y validación estructural antes de usar cualquier dato (instrucciones, sección 4). Ningún mensaje de error ni registro expone datos fiscales completos innecesarios (sección 8).

Los esquemas de CFDI 4.0, Pagos 2.0, Carta Porte 3.1 y Timbre Fiscal Digital 1.1 ya están en el juego local (ADR-03). El acuse de cancelación no tiene XSD en ese juego. La documentación del Web Service de cancelación de PAX muestra este acuse dentro de la respuesta del método `fnCancelarXML20`, el mismo que usaba CFDIAPI40:

| Parte | Contenido |
| --- | --- |
| Elemento raíz | `Cancelacion`, en el namespace `http://cancelacfd.sat.gob.mx` |
| Atributos de la raíz | `RfcEmisor` y `Fecha` |
| Elementos hijos | Uno o más `Folios`, cada uno con `UUID`, `UUIDEstatus`, `UUIDdescripcion` y `UUIDfecha`; y un `Signature` |
| Particularidades | Algunos valores traen espacios al inicio o al final (por ejemplo `201 ` o `RfcEmisor=" MAG041126GT8"`) |

El acuse que sube el cliente lo genera su sistema actual, que puede usar otro PAC o el servicio del SAT con una forma distinta.

## Decisión

### 1. Carga segura

- El componente recibe el XML como bytes (`byte[]`), no como texto, para respetar la codificación que declara el propio archivo.
- Antes de interpretar cualquier dato, el documento se **rechaza** si:
  - mide más de **10 MB** (10,485,760 bytes);
  - contiene una DTD o entidades externas: el procesamiento de DTD está prohibido, no hay resolutor y no se admiten caracteres provenientes de entidades;
  - tiene más de **64 niveles** de anidamiento;
  - su declaración XML indica una codificación distinta de UTF-8, o sus bytes no son UTF-8 válido;
  - no es un XML bien formado.
- El tamaño se comprueba sobre los bytes, antes de leerlos. La profundidad, la DTD y la buena formación se comprueban en una **lectura de seguridad** nodo por nodo, que no construye ningún árbol en memoria. Así un documento hostil se rechaza sin agotar memoria ni pila.
- Los límites son fijos. Cambiarlos es una versión nueva del componente. Un CFDI con Carta Porte real tiene menos de 10 niveles y pocos MB.

### 2. Árbol en memoria y validación sobre el árbol

1. Después de la lectura de seguridad, el XML se carga **una sola vez** en un `XDocument`, con la misma configuración segura y conservando los números de línea.
2. Sobre ese árbol se detectan el tipo y la versión (sección 3).
3. Un documento soportado se valida contra el conjunto de esquemas que le corresponde (ADR-03). Para CFDI es el conjunto de CFDI, que ya incluye Pagos 2.0, Carta Porte 3.1, impuestos locales, leyendas fiscales y el Timbre Fiscal Digital 1.1.
4. La validación reúne **todos** los errores estructurales, no solo el primero, cada uno con su número de línea.
5. Los datos se extraen solo si la validación pasó.

### 3. Detección de tipo y versión

La detección usa el elemento raíz, su namespace y su atributo `Version`, y los namespaces de los complementos. Ocurre antes de validar: un documento no soportado no se valida.

| Caso | Resultado |
| --- | --- |
| CFDI 4.0, con o sin Pagos 2.0, Carta Porte 3.1, Timbre Fiscal Digital 1.1, impuestos locales o leyendas fiscales | Se valida y se lee |
| CFDI 4.0 con complementos fuera del alcance (por ejemplo Comercio Exterior o nómina) | Se valida y se lee. Esos complementos no se extraen y se listan como "no leídos" |
| CFDI 3.3 o anterior | No soportado |
| CFDI 4.0 con Carta Porte 2.0 o 3.0, con Pagos 1.0 o con otra versión del Timbre Fiscal Digital | No soportado: el documento completo, sin extraer datos (D-28) |
| Comprobante de Retenciones | No soportado: fuera del alcance de la lectura |
| Acuse de cancelación con la forma de la sección 4 | Se verifica y se lee |
| Cualquier otro XML | No reconocido |

Namespaces esperados:

| Documento o complemento | Namespace |
| --- | --- |
| CFDI 4.0 | `http://www.sat.gob.mx/cfd/4` |
| CFDI 3.3 | `http://www.sat.gob.mx/cfd/3` |
| Pagos 2.0 | `http://www.sat.gob.mx/Pagos20` |
| Carta Porte 3.1 | `http://www.sat.gob.mx/CartaPorte31` |
| Timbre Fiscal Digital 1.1 | `http://www.sat.gob.mx/TimbreFiscalDigital` |
| Acuse de cancelación | `http://cancelacfd.sat.gob.mx` |

Antes del prompt de detección (TS-02-3), cada namespace de un documento soportado se confirma contra el `targetNamespace` de su XSD en `SatResources/`, con evidencia del repositorio.

### 4. Acuse de cancelación

No existe un XSD oficial del acuse en el juego local. Se valida con una **verificación estructural propia, marcada como no oficial**:

| Elemento | Regla |
| --- | --- |
| Raíz | `Cancelacion` en el namespace `http://cancelacfd.sat.gob.mx` |
| Atributos `RfcEmisor` y `Fecha` | Obligatorios |
| `Folios` | Al menos uno |
| `UUID` y `UUIDEstatus` en cada `Folios` | Obligatorios. `UUID` con formato de UUID |
| `UUIDdescripcion` y `UUIDfecha` | Opcionales |
| `Signature` | Se ignora: la lectura no verifica sellos (D-30) |

- A los valores de esos atributos y elementos se les quitan los espacios al inicio y al final. En UUID, códigos y fechas esto no cambia el significado.
- El componente entrega el código de estatus y su descripción **tal como vienen**, sin interpretarlos. El producto decide qué hacer con cada código.
- Según la tabla de códigos del Web Service de cancelación de PAX:

| Código | Significado según PAX |
| --- | --- |
| 201 | UUID cancelado sin aceptación |
| 202 | UUID con aceptación en proceso: **todavía no está cancelado** |
| 203 | UUID previamente cancelado |
| 103 | Cancelado previamente por aceptación del receptor |
| 107 | Cancelado por plazo vencido |

- El significado definitivo de cada código se confirma con PAX al firmar el nuevo contrato (TK-09-1).
- Un XML de cancelación con otra raíz o en otro namespace se reporta como no reconocido, sin fallar.
- Antes del paquete de TS-02-9, Luis comparte los nombres de elementos y atributos de un acuse real del primer cliente, sin valores. Si su forma es distinta, se agrega como segunda variante en ese mismo paquete, sin cambiar el modelo de lectura.

### 5. Modelo de lectura propio

- La lectura tiene un modelo propio, separado de los modelos de emisión. La lectura es transitoria (D-31), tolera complementos que no lee y no espera a que SP-05-1 diseñe los modelos de emisión.
- Contiene solo los datos que piden TS-02-4 a TS-02-9, no el XML completo.
- Son tipos inmutables. Las listas se exponen como colecciones de solo lectura. Un dato opcional ausente es nulo.

| Tipo de dato | Representación |
| --- | --- |
| Claves de catálogo, RFC, UUID, folios y textos | Texto exacto, tal como viene en el XML |
| Importes, cantidades, tasas y equivalencias | `decimal`, leído con cultura invariable. Conserva los 10 decimales de `EquivalenciaDR` |
| Fechas | `DateTime` sin zona horaria, tal como viene en el CFDI, sin conversiones |

- Vive en `DanmaobCfdi.Core/Reading/`, con subcarpetas por versión: `Cfdi40`, `Pagos20`, `CartaPorte31`, `Tfd11` y `Cancelacion` (ADR-01, sección 8). Los nombres exactos de los tipos los fijan los prompts de E02.

### 6. Resultado explícito, sin excepciones por documento

Toda lectura devuelve un resultado con uno de cinco estados:

| Estado | Nombre en código | Contenido |
| --- | --- | --- |
| Leído | `Read` | El tipo de documento, el modelo de lectura y la lista de complementos no leídos |
| No soportado | `Unsupported` | El tipo y la versión detectados, y el motivo |
| No reconocido | `Unrecognized` | — |
| Inválido | `Invalid` | La lista de errores estructurales, cada uno con su número de línea |
| Rechazado | `Rejected` | El motivo: tamaño, DTD o entidades, profundidad, codificación o XML mal formado |

- Un documento defectuoso nunca lanza una excepción. Las excepciones quedan para errores de programación.
- El componente no escribe en ningún registro el contenido del XML ni los datos extraídos (instrucciones, sección 8).

### 7. Claves desconocidas y descripciones

- En `CFDI_Lectura` la lectura no consulta catálogos (D-38). Ninguna clave hace rechazar un comprobante: solo cuenta el XSD.
- TS-02-10 agrega las descripciones como **un servicio aparte**: recibe un catálogo y una clave, devuelve la descripción y marca las claves desconocidas. No rechaza el comprobante.
- **El modelo de lectura no cambia** al agregar las descripciones. A partir de la versión `1.0.0`, cambiarlo sería un cambio incompatible que exigiría una versión MAYOR (ADR-02).

### 8. Forma de uso

- Un solo punto de entrada recibe los bytes de un documento y devuelve su resultado, sea un CFDI o un acuse: la detección decide.
- Un documento por llamada. El lote lo maneja el producto.
- Métodos síncronos: la lectura es solo trabajo de procesador.
- Sin estado y segura entre hilos. Reutiliza los conjuntos de esquemas compilados una sola vez (ADR-03).
- Se registra con `AddDanmaobCfdiCore` (ADR-01).

## Fuera del alcance de este ADR

- Verificar los sellos del emisor y del SAT (D-30).
- Validar claves contra catálogos o su vigencia (D-38; TS-02-10 y E07).
- Extraer complementos fuera del alcance o versiones anteriores (D-28).
- Interpretar los códigos de estatus de cancelación (pendiente de TK-09-1).
- La solicitud de cancelación a PAX (`CancelacionCFDI`): SP-09-1.

## Alternativas consideradas

| Alternativa | Por qué no se eligió |
| --- | --- |
| Deserializar con `XmlSerializer` sobre los modelos de emisión | La lectura esperaría a SP-05-1, heredaría la rigidez de la emisión ante complementos que no lee y mezclaría dos responsabilidades |
| Validar mientras se lee, en una sola pasada con un lector que valida | Hay que elegir el conjunto de esquemas antes de saber el tipo y la versión del documento |
| Comprobar la profundidad sobre el árbol ya construido | Un documento hostil ya habría consumido memoria al construirlo |
| `XmlDocument` en lugar de `XDocument` | Más pesado y con una configuración segura menos directa |
| Extraer datos sin validar antes | Contradice el principio de contenido no confiable (sección 4) |
| Rechazar comprobantes con complementos fuera del alcance | Un CFDI con, por ejemplo, Comercio Exterior es válido y su parte de CFDI le sirve al TMS |
| Extraer la parte de CFDI de un documento con Carta Porte 3.0 | Contradice D-28. El primer cliente no tiene comprobantes anteriores a 2025 |
| Rechazar el acuse por no tener XSD oficial | Dejaría sin cubrir la evidencia de cancelación (D-29) |
| Interpretar los códigos de estatus de cancelación | El significado lo fija el SAT y lo documenta PAX, pero falta confirmarlo con el nuevo contrato; mientras tanto, el producto decide con el código |
| Recibir el XML como texto | La codificación ya decodificada podría no coincidir con la que declara el archivo |
| Límites configurables por el producto | Configuración sin necesidad demostrada |
| Métodos asíncronos | No hay entrada ni salida que esperar |

## Consecuencias

**Lo que se facilita:**

- El TMS puede leer los CFDI del cliente desde `CFDI_Lectura`, sin esperar la carga de catálogos ni el diseño de la emisión.
- Un documento antiguo, desconocido o defectuoso nunca detiene la carga del cliente: cada uno devuelve su propio estado.
- Las pruebas de lectura usan los archivos de referencia del ADR-04 (`Cfdi40/`, `Pagos20/`, `CartaPorte31/`, `Cancelacion/`, `NoSoportados/` e `Invalidos/`).

**Lo que exige cuidado:**

- **Namespaces:** los de la sección 3 se confirman contra los XSD locales antes de TS-02-3.
- **Acuse de cancelación:** la verificación es propia y no oficial. TS-02-9 necesita antes la forma de un acuse real del primer cliente.
- **Códigos de cancelación:** el producto los interpreta mientras PAX no confirme su significado.
- **Modelo estable:** después de `1.0.0`, el modelo de lectura solo crece de forma compatible.

**Trabajo desbloqueado:**

| Elemento | Qué toma de este ADR |
| --- | --- |
| TS-02-1 | Carga segura y límites (sección 1) |
| TS-02-2 | Árbol y validación estructural (sección 2) |
| TS-02-3 | Detección de tipo y versión (sección 3) |
| TS-02-4 a TS-02-8 | Modelo de lectura y extracción (sección 5) |
| TS-02-9 | Acuse de cancelación (sección 4) |
| TS-02-10 | Servicio de descripciones (sección 7) |

## Historial de cambios

| Fecha | Cambio |
| --- | --- |
| 19 de octubre de 2026 | Corrección según la documentación del Web Service de cancelación de PAX: el contexto identifica el acuse como respuesta de `fnCancelarXML20` y registra espacios al inicio y al final de los valores; la sección 4 reemplaza la expectativa equivocada "201 y 202" por la tabla de códigos de PAX (202 significa aceptación en proceso, no cancelado). La decisión no cambia: el componente sigue entregando el código sin interpretarlo |
