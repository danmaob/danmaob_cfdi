# Importación y reemplazo de la copia de DANMAOB_CFDI en un producto

Procedimiento de TS-10-1, según el ADR-02 (secciones 4 y 5). Lo ejecuta Luis.

## Qué hace

El componente se usa como copia dentro de la solución de cada producto (DANMAOB_TMS, DANMAOB_ERP). La copia vive en la carpeta `DanmaobCfdi/`, junto al archivo de solución del producto, y su contenido es exactamente `src/` del original en una etiqueta de entrega (`vX.Y.Z`). Nunca se edita dentro del producto: cuando cambia el original, se reemplaza completa.

`tools/copia/reemplazar-copia.sh` hace las dos operaciones:

- **Primera importación.** Si el producto no tiene `DanmaobCfdi/`, la crea y agrega a su solución `DanmaobCfdi.Core`, `DanmaobCfdi.Catalogs` y `DanmaobCfdi.Pax`. Las referencias desde los proyectos del producto (por ejemplo, el módulo de facturación hacia `DanmaobCfdi.Core`) las agrega el propio producto.
- **Reemplazo.** Si ya existe, comprueba que la copia sea idéntica a la etiqueta de la versión que declara y la reemplaza completa por la nueva.

En los dos casos el script:

1. Comprueba que la etiqueta exista en el original y que su versión no lleve `-dev`.
2. Se detiene si el producto tiene cambios sin commit.
3. Se detiene si la copia actual fue editada dentro del producto, o si la etiqueta pedida es anterior a la versión actual (un producto nunca baja de versión).
4. Coloca la copia desde la etiqueta (`git archive`), nunca desde el árbol de trabajo, y comprueba que sea idéntica.
5. Compila la solución del producto.
6. Hace un commit en la rama actual del producto con la versión en el mensaje, sin push: el producto decide cuándo subirlo.

Si la solución no compila, no hace el commit y muestra el comando que deja el producto como estaba.

## Antes de ejecutarlo

- La versión está entregada: existe la etiqueta `vX.Y.Z` en el original (la primera será `v1.0.0`, en TS-10-4).
- El repositorio del producto no tiene cambios sin commit y su `.gitignore` ignora `bin/` y `obj/`.
- El producto usa el SDK de .NET 10 y, en los paquetes que comparte con la copia (EF Core y `Microsoft.Extensions`), la misma versión mayor y un parche igual o mayor (ADR-02, sección 7).
- Docker no hace falta.

## Comando

Un solo comando, con dos datos: el archivo de solución del producto y la etiqueta. Por ejemplo, para la versión `v1.0.0` en un producto cuya solución fuera `/Users/luisortiz/Desarrollo/DANMAOB/DANMAOB_TMS/Net/DanmaobTms.slnx`:

```
"/Users/luisortiz/Desarrollo/DANMAOB/DANMAOB_CFDI/Net/tools/copia/reemplazar-copia.sh" "/Users/luisortiz/Desarrollo/DANMAOB/DANMAOB_TMS/Net/DanmaobTms.slnx" v1.0.0
```

La ruta real de cada producto se confirma en su primera importación (precisión al ADR-02 del 3 de noviembre de 2026). Claude da el comando exacto en cada entrega.

La salida termina con `RESULTADO: copia vX.Y.Z lista en ...` o con una línea `DETENIDO:` que dice por qué no hizo nada.

## Después del reemplazo

- Revisar el commit del producto y subirlo según el flujo de ramas del producto.
- Si la versión trae migraciones del esquema `sat`, aplicarlas en el despliegue del producto (ADR-02, sección 6; TS-10-2).
- Consultar `CHANGELOG.md` del original: el apartado "Incompatible" dice si el producto debe cambiar su código.

## Prueba del procedimiento

`tools/copia/probar-reemplazo.sh` prueba el script sin tocar el original ni ningún producto real: clona el original en una carpeta temporal, crea ahí las etiquetas de prueba `v9.9.1`, `v9.9.2` y `v9.9.3` (esta última con `-dev`), arma un producto mínimo y comprueba seis casos: primera importación, misma versión otra vez, reemplazo, copia editada, versión con `-dev` y bajada de versión. Termina con `RESULTADO: prueba del reemplazo correcta.` Se vuelve a ejecutar cada vez que cambie `reemplazar-copia.sh`.
