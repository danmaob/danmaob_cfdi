# ADR-02 — Versionado del original y de las copias

**Estado:** Aceptado
**Fecha:** 17 de octubre de 2026
**Historia:** SP-01-2 (épica E01 — Fundamentos del componente)
**Requerimientos:** P02; D-8; D-16; D-19; D-25
**Depende de:** ADR-01 (estructura de la solución)

## Contexto

DANMAOB_CFDI tiene un original único. Cada producto lleva una copia que no se edita y que se reemplaza completa cuando cambia el original (D-8). Según el ADR-01, la unidad de copia es la carpeta `Net/src/`.

Los datos de catálogos del SAT se actualizan junto con la versión del componente (D-16). Lo mismo ocurre con los datos fijos de `c_MotivoCancelacion` y `c_TipoCadenaPago` (D-19 y D-25). El esquema `sat` tiene sus propias migraciones y su propia tabla de historial (D-9), y el producto no mapea ni modifica esas tablas.

Sin un esquema de versión no se puede responder:

- qué versión del original tiene cada producto;
- si una versión nueva obliga al producto a cambiar su código;
- qué migraciones y qué datos de catálogos le faltan a la base de un producto.

Hay además dos riesgos técnicos que este ADR debe cerrar:

- El SDK de .NET 8 y posteriores agrega el hash del commit a la versión informativa del ensamblado. Dentro de un producto, ese hash sería el del repositorio del producto, no el del original.
- Si un producto referencia una versión de un paquete *menor* que la que declara la copia, NuGet marca una degradación de paquete (NU1605), que por omisión es un error de restauración.

## Decisión

### 1. Esquema de versión

Versión semántica `MAYOR.MENOR.PARCHE`:

| Sube | Cuando |
| --- | --- |
| MAYOR | El producto debe cambiar su código: cambió el contrato público de forma incompatible (interfaces, modelos, tipos de resultado o métodos de registro), o cambió la versión mayor de .NET o de EF Core |
| MENOR | Hay funcionalidad nueva compatible: un comprobante, un complemento, un catálogo, un juego nuevo de esquemas del SAT o un cambio de estructura del esquema `sat` que no toca el contrato público |
| PARCHE | Hay correcciones o datos nuevos de catálogos (una nueva publicación del SAT) sin cambio de contrato |

Si una entrega reúne cambios de varios tipos, manda el de mayor nivel.

La primera versión es `0.1.0`. Mientras la versión mayor sea `0`, un cambio incompatible sube MENOR. La primera entrega al TMS (TS-10-4, versión de corrección `CFDI_Lectura`) es `1.0.0`, y desde ahí la tabla se aplica completa.

Las versiones de corrección de Jira (`CFDI_Lectura`, `CFDI_Emision_TMS`, `CFDI_ERP`, `CFDI_Retenciones`) no son números de versión. Cada una se cierra con una versión etiquetada del componente.

### 2. Fuente única de la versión

La versión se escribe una sola vez, en la propiedad `Version` de `src/Directory.Build.props`. Los tres proyectos de producción la heredan, y viaja con la copia porque ese archivo es parte de la unidad de copia.

`src/Directory.Build.props` fija además `IncludeSourceRevisionInInformationalVersion` en `false`, para que la versión informativa del ensamblado sea exactamente el valor de `Version`, sin hash de commit.

`AssemblyVersion` y `FileVersion` no se declaran: el SDK los deriva de `Version` sin el sufijo. TS-01-3 lo comprueba con una prueba.

`DanmaobCfdi.Core` expone una propiedad pública de solo lectura que devuelve la versión leída del atributo de versión informativa del ensamblado, sin repetir el número en el código. Su nombre lo fija TS-01-3.

### 3. Desarrollo, entrega y etiquetas

1. Durante el desarrollo, `Version` contiene la siguiente versión con el sufijo `-dev` (por ejemplo `0.1.0-dev`). TS-01-1 crea `src/Directory.Build.props` con `0.1.0-dev`.
2. Las ramas de sprint no cambian la versión.
3. Una entrega se hace en `main`, después de integrar la rama del sprint, con un commit que quita el sufijo `-dev` y actualiza `CHANGELOG.md`.
4. Ese commit recibe una etiqueta anotada `vX.Y.Z` (por ejemplo `v1.0.0`), que se sube al remoto.
5. El commit siguiente cambia `Version` a la próxima versión con `-dev`.
6. Una versión con `-dev` nunca se entrega a un producto.

Claude da a Luis los comandos exactos de cada entrega.

### 4. Lugar de la copia en el producto

La copia vive en una carpeta `DanmaobCfdi/`, en la raíz de la solución de cada producto, junto a su archivo de solución. Su contenido es exactamente el de `Net/src/` del original en la etiqueta entregada.

La ruta concreta en DANMAOB_TMS y en DANMAOB_ERP se confirma con evidencia de cada repositorio en TS-10-1.

### 5. Reglas del reemplazo

TS-10-1 convierte estas reglas en un procedimiento con comandos exactos:

1. La copia sale siempre de una etiqueta del original (`git archive` de `src/` en `vX.Y.Z`), nunca del árbol de trabajo.
2. Antes de reemplazar, se comprueba que la copia actual del producto sea idéntica a la etiqueta de la versión que declara. Si alguien la editó dentro del producto, el procedimiento se detiene.
3. La carpeta `DanmaobCfdi/` se borra y se reemplaza completa.
4. Después del reemplazo se verifica que la versión no tenga `-dev`, que la copia sea idéntica a la etiqueta y que la solución del producto compile.
5. El commit en el producto lleva la versión del componente en el mensaje.

### 6. Migraciones y datos de catálogos

- Las migraciones del esquema `sat`, de estructura y de datos, viven en `DanmaobCfdi.Catalogs` y viajan con la copia. Los datos fijos de `c_MotivoCancelacion` y `c_TipoCadenaPago` siguen el mismo camino.
- El producto las aplica como paso explícito de su despliegue, con `dotnet ef database update` sobre el `DbContext` del componente. El componente nunca aplica migraciones por sí solo al arrancar.
- Las migraciones solo avanzan. Un producto nunca baja de versión del componente.
- Lo que le falta a una base se sabe con dos fuentes:
  - la tabla de historial propia del esquema `sat`, que dice qué migraciones tiene;
  - una tabla de versiones cargadas que registra, por catálogo, la publicación del SAT, la versión del componente que la cargó y la fecha de aplicación.
- La estructura de la tabla de versiones cargadas la fija SP-03-1; su llenado, TS-04-3; y la forma en que los datos se convierten en migraciones, SP-04-1.

### 7. Alineación de paquetes con el producto

- La copia declara, en cada `.csproj`, la versión mínima de cada paquete con la que se probó.
- El producto usa la misma versión mayor de cada paquete compartido (EF Core y las abstracciones de `Microsoft.Extensions`) y un parche igual o mayor que el de la copia.
- El componente sube el parche de sus paquetes solo cuando lo necesita, y lo anota en `CHANGELOG.md`.

### 8. Registro de cambios

`Net/CHANGELOG.md` vive en la raíz del repositorio y no viaja con la copia. Tiene una sección por versión entregada, de la más reciente a la más antigua, con estos apartados cuando apliquen:

| Apartado | Contenido |
| --- | --- |
| Agregado | Funcionalidad nueva |
| Cambiado | Cambios compatibles de comportamiento |
| Corregido | Defectos corregidos |
| Incompatible | Lo que obliga al producto a cambiar su código, con la indicación de cómo |
| Migraciones | Migraciones del esquema `sat` que trae la versión |
| Catálogos | Catálogos actualizados, con su publicación del SAT |
| Paquetes | Versiones de paquetes que cambiaron |

Lo redacta Claude y Luis lo incluye en el commit de entrega.

## Fuera del alcance de este ADR

- Cómo conviven dos juegos de esquemas del SAT dentro del componente: SP-01-3.
- Estructura de la tabla de versiones cargadas: SP-03-1.
- Conversión de los Excel del SAT en datos versionados: SP-04-1.

## Alternativas consideradas

| Alternativa | Por qué no se eligió |
| --- | --- |
| Versión por fecha (por ejemplo `2026.10.17`) | No dice si una versión obliga al producto a cambiar su código |
| Archivo `VERSION` aparte | Sería una segunda fuente que hay que mantener igual a la del ensamblado, o leerla durante la compilación |
| Versión solo en las etiquetas de Git | La copia no lleva `.git`: el producto no tendría forma de saber qué versión contiene |
| Hash del commit en la versión informativa | Dentro del producto, el hash sería el del repositorio del producto |
| Entregar desde una rama o desde el árbol de trabajo | No es reproducible: dos entregas de la "misma" versión podrían diferir |
| Aplicar las migraciones al arrancar | El componente decidiría cuándo cambia la base del producto, con riesgo cuando arrancan varias instancias a la vez |
| Migraciones reversibles para bajar de versión | Complejidad sin necesidad: los datos de catálogos solo avanzan con el SAT |
| Manifiesto de hashes dentro de la copia | Redundante: comparar la copia contra su etiqueta da la misma certeza sin archivos adicionales |
| Versiones de paquetes idénticas a las del producto | Obligaría a entregar una copia nueva por cada parche que el producto adopte |

## Consecuencias

**Lo que se facilita:**

- La versión de cada copia se lee en `DanmaobCfdi/Directory.Build.props` del producto o, en ejecución, con la propiedad de `Core`.
- Una edición de la copia dentro de un producto se detecta antes del siguiente reemplazo.
- Cada entrega es reproducible desde su etiqueta.
- El producto sabe, por `CHANGELOG.md`, si una versión le exige cambiar código, aplicar migraciones o alinear paquetes.

**Lo que exige cuidado:**

- Cada entrega sigue los pasos de la sección 3 en orden; una etiqueta sobre un commit con `-dev` es un error.
- Las migraciones del esquema `sat` nunca se editan después de entregadas: una corrección es una migración nueva.
- El producto debe mantener sus paquetes compartidos en la misma versión mayor que la copia.

**Trabajo desbloqueado:**

- TS-01-3: marca de versión del componente.
- TS-10-1: procedimiento de reemplazo de la copia.
- SP-03-1 y SP-04-1, que parten de la tabla de versiones cargadas y del principio de que los datos viajan como migraciones.
- TS-10-2: aplicación de las migraciones del esquema `sat` en el producto.
