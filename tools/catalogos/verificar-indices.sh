#!/bin/bash
# DANMAOB_CFDI: verifica en la base local las llaves principales, los indices, las intercalaciones y las
# migraciones del esquema sat contra indices-esperados.txt (ADR-06, secciones 4 y 9; TS-03-4).
#
# Uso: verificar-indices.sh
#   Lee el usuario y la contrasena de SQL Server de DANMAOB_SQL_USUARIO y DANMAOB_SQL_CONTRASENA si vienen
#   en el mismo comando; si no, los pide sin mostrarlos. No guarda nada y no modifica la base: solo consulta
#   el catalogo de SQL Server con sqlcmd dentro del contenedor danmaob-sqlserver.
# Termina con "RESULTADO: llaves, indices e intercalaciones correctos." o con las diferencias.
# Solo caracteres ASCII.

AQUI="$(cd "$(dirname "$0")" && pwd)"
ESPERADO="$AQUI/indices-esperados.txt"
CONTENEDOR="danmaob-sqlserver"
SQLCMD="/opt/mssql-tools18/bin/sqlcmd"
BASE="DanmaobCfdi"

[ -f "$ESPERADO" ] || { echo "No existe $ESPERADO"; exit 1; }
docker ps --format '{{.Names}}' 2>/dev/null | grep -q "^$CONTENEDOR\$" || { echo "DETENIDO: el contenedor $CONTENEDOR no esta encendido. Enciende Docker y el contenedor, y vuelve a ejecutar."; exit 1; }

USUARIO="$DANMAOB_SQL_USUARIO"
CONTRASENA="$DANMAOB_SQL_CONTRASENA"
if [ -z "$USUARIO" ] || [ -z "$CONTRASENA" ]; then
    read -r -p "Usuario de SQL Server: " USUARIO
    read -r -s -p "Contrasena de SQL Server: " CONTRASENA
    echo
fi

CONSULTA="SET NOCOUNT ON;
SELECT 'MIG|' + MigrationId FROM sat.__EFMigrationsHistory
UNION ALL
SELECT 'KEY|' + t.name + '|' + STRING_AGG(c.name, ',') WITHIN GROUP (ORDER BY ic.key_ordinal)
FROM sys.indexes i
JOIN sys.tables t ON t.object_id = i.object_id
JOIN sys.schemas s ON s.schema_id = t.schema_id
JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id AND ic.key_ordinal > 0
JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
WHERE s.name = 'sat' AND i.is_primary_key = 1 AND t.name <> '__EFMigrationsHistory'
GROUP BY t.name
UNION ALL
SELECT 'INDEX|' + t.name + '|' + STRING_AGG(c.name, ',') WITHIN GROUP (ORDER BY ic.key_ordinal) + '|' + CAST(i.is_unique AS varchar(1)) + CASE WHEN i.has_filter = 1 THEN '|FILTRO' ELSE '' END
FROM sys.indexes i
JOIN sys.tables t ON t.object_id = i.object_id
JOIN sys.schemas s ON s.schema_id = t.schema_id
JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id AND ic.key_ordinal > 0
JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
WHERE s.name = 'sat' AND i.is_primary_key = 0 AND i.type > 0 AND t.name <> '__EFMigrationsHistory'
GROUP BY t.name, i.name, i.is_unique, i.has_filter
UNION ALL
SELECT 'COLL|' + t.name + '|' + c.name + '|' + c.collation_name
FROM sys.columns c
JOIN sys.tables t ON t.object_id = c.object_id
JOIN sys.schemas s ON s.schema_id = t.schema_id
WHERE s.name = 'sat' AND c.collation_name = 'Latin1_General_100_CI_AI' AND t.name <> '__EFMigrationsHistory';"

TMP="$(mktemp -d)"
if ! docker exec -i -e SQLCMDPASSWORD="$CONTRASENA" "$CONTENEDOR" "$SQLCMD" -S localhost -U "$USUARIO" -C -d "$BASE" -b -h -1 -W -Q "$CONSULTA" > "$TMP/salida.txt" 2>&1; then
    sed 's/^/        /' "$TMP/salida.txt" | head -n 20
    rm -rf "$TMP"
    echo "DETENIDO: la consulta a la base $BASE fallo."
    exit 1
fi
CONTRASENA=""

sed -E 's/\r$//; s/[[:space:]]+$//; s/^MIG\|[0-9]{14}_/MIG|/' "$TMP/salida.txt" | grep -E '^(MIG|KEY|INDEX|COLL)\|' | LC_ALL=C sort > "$TMP/real.txt"
grep -E '^(MIG|KEY|INDEX|COLL)\|' "$ESPERADO" | LC_ALL=C sort > "$TMP/esperado.txt"

echo "== Esquema sat de la base $BASE"
for tipo in MIG KEY INDEX COLL; do
    printf '  %-6s esperados: %3d  encontrados: %3d\n' "$tipo" "$(grep -c "^$tipo|" "$TMP/esperado.txt")" "$(grep -c "^$tipo|" "$TMP/real.txt")"
done
if diff -q "$TMP/esperado.txt" "$TMP/real.txt" > /dev/null; then
    rm -rf "$TMP"
    echo
    echo "RESULTADO: llaves, indices e intercalaciones correctos."
    exit 0
fi
echo
echo "Diferencias (- falta en la base, + sobra en la base):"
diff "$TMP/esperado.txt" "$TMP/real.txt" | grep -E '^[<>]' | sed 's/^</  -/; s/^>/  +/'
rm -rf "$TMP"
echo
echo "RESULTADO: el esquema sat no coincide con lo esperado. Pega esta salida en el chat."
exit 1
