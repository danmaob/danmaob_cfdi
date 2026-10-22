#!/bin/bash
# Genera un archivo de referencia de CFDI a partir de una plantilla (ADR-04, secciones 2 y 4):
#   <salida>.xml         el CFDI completo y sellado, validado con xmllint contra todos los esquemas de CFDI
#                        (esquemas-cfdi.xsd), así que puede llevar complementos soportados, como el timbre
#   <salida>.cadena.txt  la cadena original calculada por xsltproc
#   <salida>.sello.txt   el sello calculado por OpenSSL (SHA-256 con RSA, base 64)
# Los valores esperados salen de herramientas ajenas a .NET, nunca del componente.
# Uso: generar-referencia.sh <plantilla> <RFC del CSD> <ruta de salida sin extensión>
# La llave privada solo se descifra en una carpeta temporal que se borra al terminar.
set -euo pipefail
DIR="$(cd "$(dirname "$0")" && pwd)"
REPO="$(cd "$DIR/../.." && pwd)"
SAT="$REPO/src/DanmaobCfdi.Core/SatResources"
CSD="$REPO/tests/DanmaobCfdi.Tests/Fixtures/Csd"
XSLT="$SAT/sitio_internet/cfd/4/cadenaoriginal_4_0/cadenaoriginal_4_0.xslt"
XSD="$DIR/esquemas-cfdi.xsd"
export XML_CATALOG_FILES="$DIR/catalogo-sat.xml"

if [ $# -ne 3 ]; then echo "Uso: $0 <plantilla> <RFC> <salida sin extensión>" >&2; exit 2; fi
PLANTILLA="$1"; RFC="$2"; SALIDA="$3"
CER="$CSD/$RFC.cer"; KEY="$CSD/$RFC.key"; PASS="$CSD/$RFC.password.txt"
for f in "$PLANTILLA" "$CER" "$KEY" "$PASS" "$XSLT" "$XSD"; do
    [ -f "$f" ] || { echo "No existe: $f" >&2; exit 1; }
done

umask 077
TMP="$(mktemp -d)"
trap 'rm -rf "$TMP"' EXIT

# Número de certificado: el serial del .cer, en hexadecimal, leído como texto
SERIAL_HEX="$(openssl x509 -inform DER -in "$CER" -noout -serial | cut -d= -f2)"
NO_CERTIFICADO="$(perl -e 'print pack("H*", $ARGV[0])' "$SERIAL_HEX")"
# Certificado: el .cer en base 64, sin saltos de línea
CERTIFICADO="$(openssl base64 -A -in "$CER" | tr -d '\n')"

NO_CERTIFICADO="$NO_CERTIFICADO" CERTIFICADO="$CERTIFICADO" perl -pe 's/\{\{NO_CERTIFICADO\}\}/$ENV{NO_CERTIFICADO}/g; s/\{\{CERTIFICADO\}\}/$ENV{CERTIFICADO}/g' "$PLANTILLA" > "$TMP/sin-sello.xml"

# Cadena original con la XSLT oficial local, sin red
xsltproc --nonet "$XSLT" "$TMP/sin-sello.xml" > "$TMP/cadena.txt"
[ -s "$TMP/cadena.txt" ] || { echo "xsltproc produjo una cadena vacía" >&2; exit 1; }

# Sello: SHA-256 con RSA sobre la cadena, con la llave del CSD de pruebas
head -n 1 "$PASS" | LC_ALL=C tr -d '\r\n' > "$TMP/contrasena.txt"
openssl pkcs8 -inform DER -in "$KEY" -passin "file:$TMP/contrasena.txt" -out "$TMP/llave.pem"
openssl dgst -sha256 -sign "$TMP/llave.pem" -out "$TMP/firma.bin" "$TMP/cadena.txt"
openssl base64 -A -in "$TMP/firma.bin" | tr -d '\n' > "$TMP/sello.txt"

SELLO="$(cat "$TMP/sello.txt")" perl -pe 's/\{\{SELLO\}\}/$ENV{SELLO}/g' "$TMP/sin-sello.xml" > "$TMP/final.xml"
if grep -q '{{' "$TMP/final.xml"; then echo "Quedó un marcador sin reemplazar" >&2; exit 1; fi

# Comprobaciones: XSD, cadena del XML final y firma contra la llave pública del certificado
xmllint --nonet --noout --schema "$XSD" "$TMP/final.xml"
xsltproc --nonet "$XSLT" "$TMP/final.xml" > "$TMP/cadena-final.txt"
cmp -s "$TMP/cadena.txt" "$TMP/cadena-final.txt" || { echo "La cadena del XML final no coincide" >&2; exit 1; }
openssl x509 -inform DER -in "$CER" -noout -pubkey > "$TMP/publica.pem"
openssl dgst -sha256 -verify "$TMP/publica.pem" -signature "$TMP/firma.bin" "$TMP/cadena.txt" > /dev/null

mkdir -p "$(dirname "$SALIDA")"
cp "$TMP/final.xml" "$SALIDA.xml"
cp "$TMP/cadena.txt" "$SALIDA.cadena.txt"
cp "$TMP/sello.txt" "$SALIDA.sello.txt"
echo "Generado: $SALIDA.xml, $SALIDA.cadena.txt y $SALIDA.sello.txt (certificado $NO_CERTIFICADO)"
