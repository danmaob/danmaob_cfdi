# ADR-01 — Estructura de la solución `Net`

**Estado:** Aceptado
**Fecha:** 17 de octubre de 2026
**Historia:** SP-01-1 (épica E01 — Fundamentos del componente)
**Requerimientos:** P01; D-7; D-8; D-23; §5.1; §5.2
**Depende de:** —

## Contexto

`Net` solo contiene el `.gitignore` inicial. Ningún prompt puede escribirse sin rutas, proyectos y namespaces exactos, así que esta es la primera decisión del componente.

DANMAOB_CFDI es una biblioteca de clases .NET 10 (D-7) que se incluye como copia en la solución de DANMAOB_TMS y de DANMAOB_ERP. El original es único, las copias no se editan y se reemplazan completas (D-8). Se descartó que vuelva a ser una API (D-23).

El componente tiene cuatro tipos de dependencias:

| Tipo | Funciones | Dependencia externa |
| --- | --- | --- |
| Solo .NET | Lectura y validación de XML, cadena original (XSLT), sellado, modelos y cálculos | Ninguna |
| Base de datos | Catálogos del SAT en el esquema `sat` | EF Core con SQL Server |
| Red | Timbrado y cancelación con PAX | Clientes WCF (`System.ServiceModel`) |
| Desarrollo | Pruebas y reglas de arquitectura | xUnit y NetArchTest.Rules |

La distribución por copia trae un riesgo propio. MSBuild busca `Directory.Build.props` y `Directory.Packages.props` hacia arriba en las carpetas. Una copia que no lleve los suyos se compilaría con las reglas del producto que la contiene, y podría comportarse distinto que el original.

Además, las versiones del SAT deben quedar aisladas, de modo que agregar o actualizar un comprobante, complemento o catálogo no obligue a reescribir el resto (instrucciones, sección 4).

## Decisión

### 1. Proyectos separados por dependencia

La solución tiene tres proyectos de producción y uno de pruebas. Se separan por dependencia externa, no por capa: el componente no tiene dominio de negocio que justifique capas.

| Proyecto | Contenido | Referencias permitidas |
| --- | --- | --- |
| `DanmaobCfdi.Core` | Recursos oficiales del SAT, modelos, lectura y validación de XML, cadena original, sellado, cálculos fiscales, interfaz de acceso a catálogos e interfaz del PAC | Solo .NET y las abstracciones de `Microsoft.Extensions`: `DependencyInjection.Abstractions`, `Options` y `Logging.Abstractions` |
| `DanmaobCfdi.Catalogs` | `DbContext` del esquema `sat`, sus migraciones e implementación de la consulta de catálogos | `DanmaobCfdi.Core` y EF Core con SQL Server |
| `DanmaobCfdi.Pax` | Adaptador de PAX con sus clientes WCF | `DanmaobCfdi.Core` y los paquetes de `System.ServiceModel` |
| `DanmaobCfdi.Tests` | Pruebas xUnit y reglas de arquitectura | Los tres proyectos de producción, xUnit y NetArchTest.Rules |

SP-01-4 (estrategia de pruebas) puede dividir el proyecto de pruebas si lo justifica.

### 2. Dirección de dependencias y reglas de arquitectura

`Core` no referencia a ningún otro proyecto de la solución. `Catalogs` y `Pax` implementan interfaces de `Core` y no se referencian entre sí.

Las pruebas de arquitectura, con NetArchTest.Rules, verifican:

1. `DanmaobCfdi.Core` no depende de `Microsoft.EntityFrameworkCore`, `Microsoft.Data.SqlClient` ni `System.ServiceModel`.
2. `DanmaobCfdi.Catalogs` no depende de `DanmaobCfdi.Pax` ni de `System.ServiceModel`.
3. `DanmaobCfdi.Pax` no depende de `DanmaobCfdi.Catalogs` ni de `Microsoft.EntityFrameworkCore`.

Así, la lectura de XML, el sellado y la cadena original se prueban sin base de datos ni red.

### 3. Estructura de carpetas

```
Net/
├── .gitignore
├── DanmaobCfdi.slnx
├── README.md
├── CLAUDE.md                     (TS-01-2)
├── docs/
│   └── adr/
├── src/                          unidad de copia (sección 4)
│   ├── Directory.Build.props
│   ├── Directory.Packages.props
│   ├── .editorconfig
│   ├── DanmaobCfdi.Core/
│   │   └── SatResources/
│   ├── DanmaobCfdi.Catalogs/
│   └── DanmaobCfdi.Pax/
├── tests/
│   ├── Directory.Build.props
│   ├── .editorconfig
│   └── DanmaobCfdi.Tests/
└── tools/                        reservada; SP-04-1 decide si la usa
```

Cada proyecto vive en una carpeta con su mismo nombre, y su archivo de proyecto se llama igual (`src/DanmaobCfdi.Core/DanmaobCfdi.Core.csproj`).

### 4. Unidad de copia

Lo que se copia a cada producto es la carpeta `Net/src/` completa, incluidos sus archivos de configuración. `tests/`, `tools/`, `docs/`, `README.md` y `CLAUDE.md` no viajan.

En el producto, la copia ocupa una carpeta propia y el producto solo agrega a su solución las referencias a los proyectos que use. La ruta dentro de cada producto y el procedimiento para reemplazar la copia los define SP-01-2 (versionado).

### 5. Configuración de compilación

`src/Directory.Build.props` fija para los tres proyectos de producción:

| Propiedad | Valor |
| --- | --- |
| `TargetFramework` | `net10.0` |
| `LangVersion` | `latest` |
| `Nullable` | `enable` |
| `WarningsAsErrors` | `Nullable` |
| `ImplicitUsings` | `disable`: cada archivo declara sus `using`, lo que hace verificables los prompts |
| `EnforceCodeStyleInBuild` | `true` |

Los archivos `.csproj` no repiten estas propiedades.

`src/.editorconfig` se declara raíz (`root = true`), para no heredar el del producto, y convierte en error de compilación dos reglas de estilo:

- IDE0161: namespace declarado con punto y coma, nunca con llaves.
- IDE0040: modificador de acceso explícito.

Las claves exactas de ambos archivos las fija TS-01-1.

`tests/Directory.Build.props` importa `../src/Directory.Build.props` y solo agrega lo propio de las pruebas. `tests/.editorconfig` es una copia idéntica de `src/.editorconfig`, porque un `.editorconfig` raíz no alcanza a una carpeta hermana.

### 6. Versiones de paquetes

Cada `.csproj` declara la versión exacta de sus paquetes. No se usa administración central de paquetes, para que la copia no dependa de archivos del producto.

`src/Directory.Packages.props` solo desactiva la administración central (`ManagePackageVersionsCentrally` en `false`). Su función es impedir que la copia herede la del producto, si el producto la usa, porque en ese caso las versiones declaradas en cada `.csproj` producirían errores de restauración.

La regla para alinear las versiones de EF Core y de las abstracciones de `Microsoft.Extensions` con las de cada producto la define SP-01-2.

### 7. Recursos oficiales del SAT

Los XSD y XSLT oficiales viven en `Net/src/DanmaobCfdi.Core/SatResources/`. Van en `Core` porque la lectura y la validación estructural los necesitan sin tocar la base de datos ni la red, incluidos los XSD de catálogos.

Su organización interna (subcarpetas por juego y por versión), si se incrustan en el ensamblado o se copian a la salida, y cómo se resuelven sus inclusiones los define SP-01-3.

Desde este ADR, la hard rule 2 de todos los prompts dice:

> Never modify any file under `/Users/luisortiz/Desarrollo/DANMAOB/DANMAOB_CFDI/Net/src/DanmaobCfdi.Core/SatResources/`.

### 8. Namespaces, organización interna e idioma

El namespace raíz de cada proyecto es su nombre. El namespace de cada archivo sigue su carpeta: `src/DanmaobCfdi.Core/Cfdi40/` corresponde a `DanmaobCfdi.Core.Cfdi40`. Las pruebas replican la ruta del código que prueban: `tests/DanmaobCfdi.Tests/Core/Cfdi40/` corresponde a `DanmaobCfdi.Tests.Core.Cfdi40`.

Cada versión del SAT vive en su propia carpeta, con la versión en el nombre: `Cfdi40`, `Pagos20`, `CartaPorte31`, `Tfd11`, `Retenciones20`. Agregar una versión nueva es agregar una carpeta, sin tocar las existentes. La organización de los modelos dentro de cada carpeta la define SP-05-1.

Idioma de los identificadores:

| Tipo de identificador | Idioma | Ejemplos |
| --- | --- | --- |
| Técnico: proyectos, carpetas funcionales, servicios, interfaces, resultados | Inglés | `Core`, `Catalogs`, `Pax`, `Sealing`, `Reading` |
| Vocabulario del SAT | Tal como aparece en el Anexo 20 y en los XSD | `Comprobante`, `Emisor`, `CartaPorte`, `c_FormaPago` |

El nombre de las tablas del esquema `sat` lo define SP-03-1.

### 9. Contrato público y registro

- Son públicos los contratos: interfaces, modelos y tipos de resultado.
- Las implementaciones son `internal`.
- Cada proyecto de producción expone un método de registro para la inyección de dependencias: `AddDanmaobCfdiCore`, `AddDanmaobCfdiCatalogs` y `AddDanmaobCfdiPax`.
- Cada proyecto de producción declara `InternalsVisibleTo` hacia `DanmaobCfdi.Tests`, para que las pruebas alcancen las implementaciones.

### 10. Documentación del repositorio

- Los ADR viven en `Net/docs/adr/` con el nombre `ADR-NN-<tema>.md`, numerados desde `ADR-01`. Antes de asignar un número se confirma con `ls` sobre esa carpeta.
- `Net/README.md` y `Net/CLAUDE.md` viven en la raíz del repositorio (TS-01-2).

## Alternativas consideradas

| Alternativa | Por qué no se eligió |
| --- | --- |
| Un solo proyecto | Nada impide que la lectura o el sellado dependan de EF Core o de WCF. Las pruebas del núcleo quedarían junto a la configuración de la base y del PAC |
| Capas (Domain, Application, Infrastructure) | Sobreingeniería: el componente no tiene dominio de negocio, solo funciones fiscales sobre datos que recibe |
| Un proyecto por comprobante o complemento | Multiplica los proyectos que cada producto debe referenciar en cada copia. Las carpetas por versión dan el mismo aislamiento |
| Proyecto separado para las interfaces | No aporta: `Core` ya no tiene dependencias externas, así que las interfaces pueden vivir ahí |
| Proyecto separado para los recursos del SAT | `Core` siempre los necesita, así que solo agregaría un proyecto más a cada copia |
| `Directory.Build.props` en la raíz de `Net` | No viaja con la copia: en el producto, los proyectos tomarían el del producto |
| Administración central de paquetes | La copia dependería de un archivo del producto o chocaría con el suyo |
| Identificadores todos en inglés | Traducir el vocabulario del SAT complica cotejar el código contra el Anexo 20 y los XSD |
| Identificadores todos en español | Se aparta de las convenciones de .NET y de los productos DANMAOB |

## Consecuencias

**Lo que se facilita:**

- La lectura de XML, la cadena original y el sellado se prueban sin base de datos ni red.
- Regenerar los clientes de PAX o cambiar de PAC solo toca `DanmaobCfdi.Pax`.
- Las migraciones del esquema `sat` quedan aisladas en `DanmaobCfdi.Catalogs`.
- La copia compila igual en el original y en cada producto, porque lleva su propia configuración.
- Cada prompt tiene rutas, proyectos y namespaces exactos.

**Lo que exige cuidado:**

- Las versiones de EF Core y de `Microsoft.Extensions` de la copia deben ser compatibles con las del producto (SP-01-2).
- `tests/.editorconfig` debe mantenerse idéntico a `src/.editorconfig`. La verificación de TS-01-1 lo comprueba.
- Cada producto agrega a su solución las referencias a los proyectos que usa.

**Trabajo desbloqueado:**

- TS-01-1: solución, proyectos, `Directory.Build.props` y reglas de arquitectura.
- SP-01-2, SP-01-3 y SP-01-4, que parten de esta estructura.
- SP-03-1, que ubica el `DbContext` y las migraciones en `DanmaobCfdi.Catalogs`.
