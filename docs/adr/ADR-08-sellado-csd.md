# ADR-08 — Sellado y manejo del CSD

**Estado:** Aceptado
**Fecha:** 20 de octubre de 2026
**Historia:** SP-08-1 (épica E08 — Sellado)
**Requerimientos:** P09; §3.4; §4; §8; D-7
**Depende de:** ADR-01 (estructura de la solución), ADR-03 (recursos oficiales del SAT) y ADR-04 (estrategia de pruebas)

## Contexto

El sellado es el punto más sensible del componente: usa la llave privada del Certificado de Sello Digital (CSD) del emisor. CFDIAPI40 lo resolvía con varios defectos (informe de revisión, sección 4.2):

- `opensslkey`, `RSACryptoServiceProvider` y `SHA256Managed`, que son API obsoletas;
- la llave privada leída desde una ruta de archivo, y el certificado y su número leídos de una tabla de empresas;
- constructores obsoletos de `X509Certificate2`;
- funciones heredadas de CFDI 3.2 y 3.3 con SHA-1 y MD5.

Ya está decidido:

| Fuente | Contenido |
| --- | --- |
| §3.4 | La custodia del CSD y de su contraseña es del consumidor, no del componente |
| §8 | El componente recibe el CSD y su contraseña, los usa solo durante la operación y no los persiste; ningún error ni registro expone material criptográfico |
| §4 | Sin dominio ni tenants; resultado explícito |
| ADR-01 | El sellado vive en `DanmaobCfdi.Core`, sin base de datos ni red; contratos públicos e implementaciones `internal` |
| ADR-03 | La cadena original se calcula con la XSLT oficial local, compilada una sola vez |
| ADR-04 | Pruebas con los CSD públicos de pruebas del SAT en `Fixtures/Csd/` y valores esperados producidos por OpenSSL |

TS-01-6 dejó la evidencia que este ADR usa:

- los CSD de `EKU9003173C9` y `WERX631016S30`, con su número de certificado (`30001000000500003416` y `30001000000500003450`);
- el caso `Cfdi40/ingreso-basico`, con su cadena de `xsltproc` y su sello de OpenSSL;
- la confirmación de que la llave `.key` del SAT es PKCS#8 cifrado en DER, que OpenSSL 3 abre con la contraseña publicada.

## Decisión

### 1. Ubicación y contrato

El sellado vive en `src/DanmaobCfdi.Core/Sealing/`, namespace `DanmaobCfdi.Core.Sealing`. No depende de la versión de ningún comprobante: recibe una cadena original y devuelve un sello.

| Tipo | Visibilidad | Responsabilidad |
| --- | --- | --- |
| `CsdCredentials` | público | Los bytes del `.cer`, los bytes del `.key` y la contraseña de una operación |
| `ICsdReader` | público | Leer y validar un CSD sin sellar: el consumidor lo usa, por ejemplo, cuando el usuario sube su CSD |
| `IDigitalSealer` | público | Validar el CSD contra el emisor y la fecha del comprobante, y calcular el sello |
| `CsdInfo` | público | Datos del certificado: número de certificado, certificado en base 64, RFC del titular y vigencia |
| `SealResult`, `CsdReadResult` | público | Resultado explícito de cada operación (sección 6) |
| `CsdError` | público | Enumeración de los motivos de falla (sección 6) |
| Implementaciones | `internal` | Sin estado; registradas como singleton en `AddDanmaobCfdiCore` |

Los nombres exactos de los miembros se fijan en cada historia de E08, dentro de esta forma.

### 2. Entrada

`CsdCredentials` recibe tres valores para una sola operación:

| Valor | Tipo | Contenido |
| --- | --- | --- |
| Certificado | `ReadOnlyMemory<byte>` | El `.cer` en DER, tal como lo entrega el SAT |
| Llave privada | `ReadOnlyMemory<byte>` | El `.key`: PKCS#8 cifrado en DER |
| Contraseña | `ReadOnlyMemory<char>` | La contraseña de la llave |

Reglas:

- El componente nunca lee rutas de archivo ni variables de entorno para el CSD.
- El componente no guarda el CSD entre operaciones. No hay caché de certificados ni de llaves.
- La contraseña nunca se convierte a `string`: pasa como intervalo de caracteres directo a la importación de la llave.
- `CsdCredentials` sobrescribe `ToString` con un texto fijo sin datos, para que un registro accidental no exponga nada.

### 3. Lectura del certificado y de la llave

Solo API nativas de .NET 10:

| Elemento | API |
| --- | --- |
| Certificado | `X509CertificateLoader.LoadCertificate` sobre los bytes del `.cer` |
| Llave privada | `RSA.Create()` y `ImportEncryptedPkcs8PrivateKey(contraseña, bytes, out leídos)`; la lectura debe consumir todos los bytes |

Quedan prohibidos `RSACryptoServiceProvider`, `SHA256Managed`, los constructores de `X509Certificate2` que reciben bytes o rutas, `opensslkey`, BouncyCastle, archivos temporales y cualquier conversión a PEM.

El certificado y la llave se liberan (`Dispose`) al terminar la operación, con éxito o con falla.

### 4. Datos del certificado

| Dato | Regla |
| --- | --- |
| Número de certificado | Los bytes del número de serie, leídos como texto ASCII. Para el CSD de `EKU9003173C9` da `30001000000500003416`: el mismo valor que calcula el generador de referencias con OpenSSL |
| Certificado | Los bytes DER completos del `.cer` en base 64, sin saltos de línea |
| RFC del titular | El atributo `x500UniqueIdentifier` (OID 2.5.4.45) del sujeto. En persona moral trae `RFC / RFC del representante`: se toma el texto antes de ` / `, sin espacios |
| Vigencia | `NotBefore` y `NotAfter` del certificado, en UTC |

### 5. Validaciones

`ICsdReader` aplica las validaciones 1 a 4. `IDigitalSealer` aplica las seis antes de sellar, en este orden, y se detiene en la primera que falla:

| Orden | Validación | Código de falla |
| --- | --- | --- |
| 1 | El `.cer` se puede leer como certificado X.509 | `InvalidCertificate` |
| 2 | La contraseña abre el `.key` y la lectura consume todos sus bytes | `PrivateKeyNotOpened` |
| 3 | La llave pública del certificado es la de la llave privada (módulo y exponente iguales) | `KeyDoesNotMatchCertificate` |
| 4 | El certificado es un CSD y no una e.firma: el SAT no acepta la e.firma para sellar CFDI | `NotCsd` |
| 5 | El RFC del certificado es el RFC del emisor del comprobante, en comparación ordinal tras quitar espacios y convertir a mayúsculas | `IssuerRfcMismatch` |
| 6 | La fecha del comprobante está dentro de la vigencia | `CertificateNotYetValid` o `CertificateExpired` |

**Validación 4, CSD frente a e.firma.** La regla inicial es que el sujeto de un CSD trae el atributo `OU` con el nombre de la sucursal: los CSD de pruebas traen `OU=Sucursal 1`. La historia TS-08-3 la confirma con el `.cer` de e.firma del mismo paquete del SAT (`eku9003173c9.cer`), que se agrega a `Fixtures/Csd/` como caso negativo. Si ese certificado también trajera `OU`, la historia propone la regla del uso de llave y Luis decide antes de construirla.

**Validación 6, zona horaria.** La fecha de un CFDI no lleva zona horaria: es la hora local del lugar de expedición. El componente no adivina la zona, porque el huso horario vive en `c_CodigoPostal` y `Core` no usa la base de datos. Por eso el consumidor entrega la fecha del comprobante como `DateTimeOffset`, con su desplazamiento, y el componente compara ese instante contra la vigencia en UTC.

### 6. Resultado

Las fallas de la sección 5 son datos del usuario, no errores de programación: el TMS debe mostrarlas, por ejemplo "tu CSD venció". Por eso no son excepciones.

- `SealResult` es un éxito, con el sello y los datos del certificado (`CsdInfo`), o una falla, con un `CsdError`.
- `CsdReadResult` es un éxito, con `CsdInfo`, o una falla, con un `CsdError`.
- `CsdError` lleva solo el código. El texto que ve el usuario lo pone el consumidor.
- Ningún resultado, mensaje ni registro contiene el certificado, la llave, la contraseña ni la cadena original completa.
- Las excepciones quedan para errores de programación: argumentos nulos o una cadena original vacía.

El ADR del PAC (SP-09-1) parte de esta misma forma de resultado explícito.

### 7. Sello

- Algoritmo: RSA con SHA-256 y relleno PKCS#1 v1.5 (`RSASignaturePadding.Pkcs1`).
- Datos firmados: los bytes UTF-8 de la cadena original, sin BOM ni salto de línea final.
- Salida: base 64 sin saltos de línea.

El sello RSA con PKCS#1 v1.5 es determinista: con la misma llave y la misma cadena, .NET y OpenSSL producen el mismo resultado. Por eso la prueba lo compara carácter por carácter con `ingreso-basico.sello.txt`.

### 8. Higiene en memoria

- La contraseña y la llave pasan como intervalos directo a la importación: el componente no hace copias propias.
- Si una historia necesita un búfer propio con material sensible, lo limpia con `CryptographicOperations.ZeroMemory` antes de soltarlo.
- Los objetos `RSA` y `X509Certificate2` se crean y se liberan dentro de cada operación.

### 9. Pruebas que fija este ADR para E08

Todas sin red, con los archivos de `Fixtures/Csd/` y `Fixtures/Cfdi40/`:

| Historia | Prueba |
| --- | --- |
| TS-08-1 | El certificado de `EKU9003173C9` da el número `30001000000500003416`, el RFC `EKU9003173C9`, la vigencia de OpenSSL y el base 64 del `Certificado` de `ingreso-basico.xml`. El de `WERX631016S30` da su número y su RFC. Unos bytes que no son certificado dan `InvalidCertificate` |
| TS-08-2 | La llave se abre con la contraseña de `<RFC>.password.txt`. Una contraseña incorrecta da `PrivateKeyNotOpened` |
| TS-08-3 | La llave de `WERX631016S30` con el certificado de `EKU9003173C9` da `KeyDoesNotMatchCertificate`. El `.cer` de e.firma da `NotCsd`. Otro RFC de emisor da `IssuerRfcMismatch`. Una fecha anterior y otra posterior a la vigencia dan sus dos códigos |
| TS-08-4 | El sello de la cadena de `ingreso-basico.cadena.txt` es idéntico a `ingreso-basico.sello.txt`. `CsdCredentials.ToString` no contiene datos |

## Fuera del alcance de este ADR

- La firma de la solicitud de cancelación. Depende de la variante de cancelación de PAX (TK-09-1) y se decide en SP-09-1.
- La generación del XML y la colocación del sello en él: E05 y E06.
- La custodia del CSD en los productos.

## Alternativas consideradas

| Alternativa | Por qué se descarta |
| --- | --- |
| Conservar `opensslkey` y `RSACryptoServiceProvider` | API obsoletas; .NET 10 lee PKCS#8 cifrado de forma nativa |
| BouncyCastle | Dependencia innecesaria: .NET 10 cubre la lectura, la firma y el X.509 |
| Recibir rutas de archivo del CSD | Acopla el componente al disco del producto y contradice §8 |
| Guardar en caché el CSD ya leído | Mantendría material sensible en memoria entre operaciones; leerlo en cada sellado cuesta milisegundos |
| Excepciones para un CSD vencido o de otro RFC | Son situaciones del usuario que el consumidor debe mostrar; una excepción las confunde con fallas técnicas |
| Deducir la zona horaria del código postal | `Core` no usa la base de datos; el consumidor conoce su zona y la entrega |

## Consecuencias

**Se facilita:**

- El sellado se prueba por completo sin red ni base de datos, contra valores de OpenSSL.
- El TMS valida el CSD cuando el usuario lo sube, sin esperar al primer timbrado.
- Ningún material sensible sale de la operación que lo usa.

**Exige cuidado:**

- La regla de CSD frente a e.firma se confirma en TS-08-3 con el certificado real de e.firma.
- El consumidor debe entregar la fecha del comprobante con su desplazamiento horario correcto.
- Si el SAT cambia el algoritmo de cifrado de los `.key`, hay que comprobar que .NET lo importe. La primera prueba de TS-08-2 con los archivos reales lo confirma para el juego actual.

**Desbloquea:** TS-08-1 a TS-08-4, y la forma de resultado que usará SP-09-1.
