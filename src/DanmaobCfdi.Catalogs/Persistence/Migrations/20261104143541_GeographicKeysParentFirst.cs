using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanmaobCfdi.Catalogs.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class GeographicKeysParentFirst : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_c_Municipio",
                schema: "sat",
                table: "c_Municipio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_c_Localidad",
                schema: "sat",
                table: "c_Localidad");

            migrationBuilder.DropPrimaryKey(
                name: "PK_c_Estado",
                schema: "sat",
                table: "c_Estado");

            migrationBuilder.DropPrimaryKey(
                name: "PK_c_Colonia",
                schema: "sat",
                table: "c_Colonia");

            migrationBuilder.AddPrimaryKey(
                name: "PK_c_Municipio",
                schema: "sat",
                table: "c_Municipio",
                columns: new[] { "Estado", "Clave" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_c_Localidad",
                schema: "sat",
                table: "c_Localidad",
                columns: new[] { "Estado", "Clave" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_c_Estado",
                schema: "sat",
                table: "c_Estado",
                columns: new[] { "Pais", "Clave" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_c_Colonia",
                schema: "sat",
                table: "c_Colonia",
                columns: new[] { "CodigoPostal", "Clave" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_c_Municipio",
                schema: "sat",
                table: "c_Municipio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_c_Localidad",
                schema: "sat",
                table: "c_Localidad");

            migrationBuilder.DropPrimaryKey(
                name: "PK_c_Estado",
                schema: "sat",
                table: "c_Estado");

            migrationBuilder.DropPrimaryKey(
                name: "PK_c_Colonia",
                schema: "sat",
                table: "c_Colonia");

            migrationBuilder.AddPrimaryKey(
                name: "PK_c_Municipio",
                schema: "sat",
                table: "c_Municipio",
                columns: new[] { "Clave", "Estado" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_c_Localidad",
                schema: "sat",
                table: "c_Localidad",
                columns: new[] { "Clave", "Estado" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_c_Estado",
                schema: "sat",
                table: "c_Estado",
                columns: new[] { "Clave", "Pais" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_c_Colonia",
                schema: "sat",
                table: "c_Colonia",
                columns: new[] { "Clave", "CodigoPostal" });
        }
    }
}
