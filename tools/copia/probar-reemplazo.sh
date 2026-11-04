#!/bin/bash
# DANMAOB_CFDI: prueba de tools/copia/reemplazar-copia.sh en una carpeta temporal (TS-10-1).
#
# Uso: probar-reemplazo.sh
#
# No toca el repositorio original ni ningun producto real:
# - clona el original en una carpeta temporal y, solo en el clon, crea las etiquetas de prueba
#   v9.9.1 y v9.9.2 (versiones de entrega) y v9.9.3 (que declara 9.9.3-dev), sin subirlas;
# - crea un producto minimo (repositorio Git con una solucion .slnx vacia y su .gitignore);
# - ejecuta el reemplazo en seis casos y comprueba el resultado de cada uno.
# Termina con "RESULTADO: prueba del reemplazo correcta." o con la lista de fallas. Solo caracteres ASCII.

AQUI="$(cd "$(dirname "$0")" && pwd)"
ORIGINAL_REAL="$(cd "$AQUI/../.." && pwd)"
REEMPLAZAR="$AQUI/reemplazar-copia.sh"
TMP="$(mktemp -d)"
CLON="$TMP/original"
PRODUCTO="$TMP/producto"
SLN="$PRODUCTO/Producto.slnx"
FALLAS=0

ok() { echo "  OK    $1"; }
falla() { echo "  FALLA $1"; FALLAS=$((FALLAS + 1)); }
version_copia() { sed -n 's:.*<Version>\(.*\)</Version>.*:\1:p' "$PRODUCTO/DanmaobCfdi/Directory.Build.props" 2>/dev/null | head -n 1; }
commits_producto() { git -C "$PRODUCTO" rev-list --count HEAD; }

etiquetar() {
    sed -i.bak "s:<Version>.*</Version>:<Version>$1</Version>:" "$CLON/src/Directory.Build.props" && rm -f "$CLON/src/Directory.Build.props.bak"
    git -C "$CLON" commit -q -a -m "Prueba: version $1" && git -C "$CLON" tag -a "$2" -m "Prueba $2"
}

caso() {
    local nombre="$1" codigo_esperado="$2" texto="$3"; shift 3
    local salida codigo
    salida="$("$REEMPLAZAR" "$@" "$CLON" 2>&1)"
    codigo=$?
    echo "$salida" > "$TMP/$nombre.txt"
    if [ "$codigo_esperado" = "0" ] && [ $codigo -ne 0 ]; then falla "$nombre: termino con error ($codigo):"; echo "$salida" | tail -n 15 | sed 's/^/        /'; return; fi
    if [ "$codigo_esperado" != "0" ] && [ $codigo -eq 0 ]; then falla "$nombre: debia detenerse y termino bien"; return; fi
    if echo "$salida" | grep -F -q "$texto"; then ok "$nombre"; else falla "$nombre: no aparece \"$texto\" en la salida:"; echo "$salida" | tail -n 15 | sed 's/^/        /'; fi
}

echo "== Preparacion en $TMP"
git clone -q "$ORIGINAL_REAL" "$CLON" || { echo "No se pudo clonar el original."; exit 1; }
git -C "$CLON" config user.name "Prueba de reemplazo"
git -C "$CLON" config user.email "prueba@danmaob.local"
etiquetar "9.9.1" "v9.9.1" && etiquetar "9.9.2" "v9.9.2" && etiquetar "9.9.3-dev" "v9.9.3" || { echo "No se pudieron crear las etiquetas de prueba."; exit 1; }
ok "etiquetas de prueba v9.9.1, v9.9.2 y v9.9.3 creadas solo en el clon"
mkdir -p "$PRODUCTO" && git -C "$PRODUCTO" init -q && git -C "$PRODUCTO" config user.name "Prueba de reemplazo" && git -C "$PRODUCTO" config user.email "prueba@danmaob.local"
printf 'bin/\nobj/\n' > "$PRODUCTO/.gitignore"
(cd "$PRODUCTO" && dotnet new sln -n Producto --format slnx > /dev/null) || { echo "No se pudo crear la solucion del producto."; exit 1; }
git -C "$PRODUCTO" add -A && git -C "$PRODUCTO" commit -q -m "Producto de prueba" || { echo "No se pudo preparar el producto."; exit 1; }
ok "producto minimo con Producto.slnx"

echo
echo "== Caso 1: primera importacion de v9.9.1"
caso "caso-1" 0 "primera importacion" "$SLN" v9.9.1
[ "$(version_copia)" = "9.9.1" ] && ok "la copia declara 9.9.1" || falla "la copia declara '$(version_copia)'"
for p in DanmaobCfdi.Core DanmaobCfdi.Catalogs DanmaobCfdi.Pax; do
    grep -q "$p.csproj" "$SLN" && ok "$p esta en la solucion" || falla "$p no esta en la solucion"
done
git -C "$PRODUCTO" log --format=%s -n 1 | grep -F -q "DanmaobCfdi v9.9.1: primera importacion" && ok "commit de la primera importacion" || falla "falta el commit de la primera importacion"
[ -z "$(git -C "$PRODUCTO" status --porcelain)" ] && ok "producto sin cambios pendientes" || falla "el producto quedo con cambios pendientes"

echo
echo "== Caso 2: la misma version otra vez (no hace nada)"
ANTES="$(commits_producto)"
caso "caso-2" 0 "ya esta en la version 9.9.1" "$SLN" v9.9.1
[ "$(commits_producto)" = "$ANTES" ] && ok "sin commit nuevo" || falla "hizo un commit nuevo"

echo
echo "== Caso 3: reemplazo de v9.9.1 por v9.9.2"
caso "caso-3" 0 "reemplazo de la copia v9.9.1" "$SLN" v9.9.2
[ "$(version_copia)" = "9.9.2" ] && ok "la copia declara 9.9.2" || falla "la copia declara '$(version_copia)'"
[ "$(grep -c 'DanmaobCfdi.Core.csproj' "$SLN")" = "1" ] && ok "la solucion no repite los proyectos" || falla "la solucion repite los proyectos"

echo
echo "== Caso 4: copia editada dentro del producto"
echo "// editado dentro del producto" >> "$PRODUCTO/DanmaobCfdi/DanmaobCfdi.Core/ComponentVersion.cs"
git -C "$PRODUCTO" commit -q -a -m "Edicion indebida de la copia"
ANTES="$(commits_producto)"
caso "caso-4" 1 "fue editada dentro del producto" "$SLN" v9.9.2
[ "$(commits_producto)" = "$ANTES" ] && ok "sin commit nuevo" || falla "hizo un commit nuevo"
git -C "$PRODUCTO" reset -q --hard HEAD~1

echo
echo "== Caso 5: etiqueta que declara una version con -dev"
caso "caso-5" 1 "una version de entrega no lleva -dev" "$SLN" v9.9.3

echo
echo "== Caso 6: bajar de version"
caso "caso-6" 1 "nunca baja de version" "$SLN" v9.9.1
[ "$(version_copia)" = "9.9.2" ] && ok "la copia sigue en 9.9.2" || falla "la copia declara '$(version_copia)'"

echo
if [ "$FALLAS" -eq 0 ]; then
    rm -rf "$TMP"
    echo "RESULTADO: prueba del reemplazo correcta."
    exit 0
fi
echo "Las salidas de cada caso quedaron en $TMP"
echo "RESULTADO: prueba del reemplazo con $FALLAS fallas. Pega esta salida completa en el chat."
exit 1
