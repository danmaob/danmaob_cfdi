#!/bin/bash
# DANMAOB_CFDI: importa o reemplaza la copia del componente en la solucion de un producto (ADR-02, TS-10-1).
#
# Uso:
#   reemplazar-copia.sh <archivo de solucion del producto (.sln o .slnx)> <etiqueta vX.Y.Z> [<repositorio original>]
#
# - La copia vive en la carpeta DanmaobCfdi/, junto al archivo de solucion, y es exactamente src/ del original
#   en la etiqueta indicada (git archive), nunca el arbol de trabajo.
# - Primera importacion: crea DanmaobCfdi/ y agrega a la solucion DanmaobCfdi.Core, DanmaobCfdi.Catalogs
#   y DanmaobCfdi.Pax. Las referencias desde los proyectos del producto las agrega el propio producto.
# - Reemplazo: comprueba que la copia actual sea identica a la etiqueta de la version que declara y se
#   detiene si alguien la edito; nunca baja de version.
# - Rechaza una version con -dev, compila la solucion del producto y hace un commit en su rama actual,
#   sin push.
# El repositorio original es, por omision, el que contiene este script. Solo caracteres ASCII.

PROYECTOS="DanmaobCfdi.Core DanmaobCfdi.Catalogs DanmaobCfdi.Pax"

alto() { echo "DETENIDO: $1"; exit 1; }

SLN="$1"
ETIQUETA="$2"
ORIGINAL="${3:-$(cd "$(dirname "$0")/../.." && pwd)}"

[ -n "$SLN" ] && [ -n "$ETIQUETA" ] || { echo "Uso: reemplazar-copia.sh <solucion del producto> <vX.Y.Z> [<repositorio original>]"; exit 2; }
[ -f "$SLN" ] || alto "no existe el archivo de solucion $SLN"
case "$SLN" in *.sln|*.slnx) ;; *) alto "$SLN no es un archivo .sln ni .slnx" ;; esac
echo "$ETIQUETA" | grep -E -q '^v[0-9]+\.[0-9]+\.[0-9]+$' || alto "la etiqueta debe tener la forma vX.Y.Z (por ejemplo v1.0.0)"

PRODUCTO="$(cd "$(dirname "$SLN")" && pwd)"
SLN="$PRODUCTO/$(basename "$SLN")"
COPIA="$PRODUCTO/DanmaobCfdi"
NUEVA="${ETIQUETA#v}"

# Version declarada en src/Directory.Build.props de una etiqueta del original
version_de_etiqueta() {
    git -C "$ORIGINAL" show "$1:src/Directory.Build.props" 2>/dev/null | sed -n 's:.*<Version>\(.*\)</Version>.*:\1:p' | head -n 1
}

# Compara dos versiones X.Y.Z: imprime -1, 0 o 1
comparar_versiones() {
    local a b i x y
    IFS=. read -r -a a <<< "$1"
    IFS=. read -r -a b <<< "$2"
    for i in 0 1 2; do
        x="${a[$i]}"; y="${b[$i]}"
        if [ "$x" -lt "$y" ]; then echo -1; return; fi
        if [ "$x" -gt "$y" ]; then echo 1; return; fi
    done
    echo 0
}

# Extrae src/ de una etiqueta del original en una carpeta temporal e imprime su ruta
extraer() {
    local destino
    destino="$(mktemp -d)"
    git -C "$ORIGINAL" archive "$1" src | tar -x -C "$destino" || { rm -rf "$destino"; return 1; }
    echo "$destino/src"
}

echo "== Original"
git -C "$ORIGINAL" rev-parse --is-inside-work-tree > /dev/null 2>&1 || alto "$ORIGINAL no es un repositorio Git"
git -C "$ORIGINAL" rev-parse -q --verify "refs/tags/$ETIQUETA" > /dev/null || alto "la etiqueta $ETIQUETA no existe en $ORIGINAL"
DECLARADA="$(version_de_etiqueta "$ETIQUETA")"
[ "$DECLARADA" = "$NUEVA" ] || alto "la etiqueta $ETIQUETA declara la version '$DECLARADA' en src/Directory.Build.props; una version de entrega no lleva -dev y debe coincidir con su etiqueta"
echo "  OK    $ETIQUETA declara la version $NUEVA"

echo "== Producto"
git -C "$PRODUCTO" rev-parse --is-inside-work-tree > /dev/null 2>&1 || alto "$PRODUCTO no es un repositorio Git"
[ -z "$(git -C "$PRODUCTO" status --porcelain)" ] || alto "el producto tiene cambios sin commit; haz commit o descartalos antes del reemplazo"
echo "  OK    $PRODUCTO sin cambios pendientes"

PRIMERA=1
ANTERIOR=""
if [ -d "$COPIA" ]; then
    PRIMERA=0
    ANTERIOR="$(sed -n 's:.*<Version>\(.*\)</Version>.*:\1:p' "$COPIA/Directory.Build.props" 2>/dev/null | head -n 1)"
    echo "$ANTERIOR" | grep -E -q '^[0-9]+\.[0-9]+\.[0-9]+$' || alto "la copia actual declara la version '$ANTERIOR', que no es una version de entrega"
    git -C "$ORIGINAL" rev-parse -q --verify "refs/tags/v$ANTERIOR" > /dev/null || alto "la copia actual declara la version $ANTERIOR, pero la etiqueta v$ANTERIOR no existe en el original"
    echo "== Copia actual (v$ANTERIOR)"
    REFERENCIA="$(extraer "v$ANTERIOR")" || alto "no se pudo extraer src/ de v$ANTERIOR"
    if ! DIFERENCIAS="$(diff -r -q -x bin -x obj "$REFERENCIA" "$COPIA")"; then
        rm -rf "$(dirname "$REFERENCIA")"
        echo "$DIFERENCIAS" | sed 's/^/        /'
        alto "la copia de $PRODUCTO fue editada dentro del producto y ya no es identica a v$ANTERIOR. Las copias nunca se editan: lleva el cambio al original"
    fi
    rm -rf "$(dirname "$REFERENCIA")"
    echo "  OK    la copia es identica a v$ANTERIOR"
    case "$(comparar_versiones "$NUEVA" "$ANTERIOR")" in
        0) echo; echo "La copia ya esta en la version $NUEVA. No hay nada que hacer."; exit 0 ;;
        -1) alto "la copia esta en $ANTERIOR y la etiqueta pedida es $NUEVA: un producto nunca baja de version del componente" ;;
    esac
fi

echo "== Copia nueva ($ETIQUETA)"
NUEVO_SRC="$(extraer "$ETIQUETA")" || alto "no se pudo extraer src/ de $ETIQUETA"
rm -rf "$COPIA"
mv "$NUEVO_SRC" "$COPIA" || alto "no se pudo colocar la copia en $COPIA"
rm -rf "$(dirname "$NUEVO_SRC")"
REVISION="$(extraer "$ETIQUETA")"
diff -r -q "$REVISION" "$COPIA" > /dev/null || { rm -rf "$(dirname "$REVISION")"; alto "la copia colocada no es identica a $ETIQUETA"; }
rm -rf "$(dirname "$REVISION")"
echo "  OK    $COPIA es identica a src/ de $ETIQUETA"

if [ "$PRIMERA" -eq 1 ]; then
    echo "== Solucion del producto"
    for p in $PROYECTOS; do
        if grep -q "$p.csproj" "$SLN"; then
            echo "  OK    $p ya estaba en la solucion"
        else
            dotnet sln "$SLN" add "$COPIA/$p/$p.csproj" > /dev/null || alto "no se pudo agregar $p a $SLN"
            echo "  OK    $p agregado a la solucion"
        fi
    done
fi

echo "== Compilacion del producto"
if ! SALIDA="$(dotnet build "$SLN" --nologo 2>&1)"; then
    echo "$SALIDA" | grep -E 'error' | head -n 30 | sed 's/^/        /'
    echo
    echo "La solucion del producto no compila con la copia $ETIQUETA. Nada quedo en un commit."
    echo "Para dejar el producto como estaba:"
    echo "  git -C \"$PRODUCTO\" checkout -- . && git -C \"$PRODUCTO\" clean -fdqx -- DanmaobCfdi"
    exit 1
fi
echo "  OK    la solucion compila"
if git -C "$PRODUCTO" status --porcelain --untracked-files=all -- DanmaobCfdi | grep -E -q '/(bin|obj)/'; then
    echo
    echo "El producto no ignora las carpetas bin/ y obj/ de la copia. Nada quedo en un commit."
    echo "Agrega bin/ y obj/ al .gitignore del producto y vuelve a ejecutar este script despues de:"
    echo "  git -C \"$PRODUCTO\" checkout -- . && git -C \"$PRODUCTO\" clean -fdqx -- DanmaobCfdi"
    exit 1
fi

echo "== Commit en el producto"
if [ "$PRIMERA" -eq 1 ]; then
    MENSAJE="DanmaobCfdi $ETIQUETA: primera importacion del componente"
else
    MENSAJE="DanmaobCfdi $ETIQUETA: reemplazo de la copia v$ANTERIOR"
fi
git -C "$PRODUCTO" add -A -- DanmaobCfdi "$(basename "$SLN")" || alto "no se pudieron agregar los cambios al commit"
git -C "$PRODUCTO" commit -q -m "$MENSAJE" || alto "no se pudo hacer el commit en el producto"
echo "  OK    $(git -C "$PRODUCTO" log --format=%s -n 1)"
echo
echo "RESULTADO: copia $ETIQUETA lista en $COPIA. El commit quedo en la rama $(git -C "$PRODUCTO" branch --show-current) del producto, sin push."
