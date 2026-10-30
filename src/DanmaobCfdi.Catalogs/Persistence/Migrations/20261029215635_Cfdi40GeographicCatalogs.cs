using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanmaobCfdi.Catalogs.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Cfdi40GeographicCatalogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "c_ClaveProdServ",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    IncluirIvaTrasladado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IncluirIepsTrasladado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ComplementoQueDebeIncluir = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EstimuloFranjaFronteriza = table.Column<byte>(type: "tinyint", nullable: false),
                    PalabrasSimilares = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true, collation: "Latin1_General_100_CI_AI"),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_ClaveProdServ", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_ClaveUnidad",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    DescripcionUnidad = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Nota = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Simbolo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_ClaveUnidad", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_CodigoPostal",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Municipio = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Localidad = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    EstimuloFranjaFronteriza = table.Column<byte>(type: "tinyint", nullable: false),
                    HusoHorario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MesInicioHorarioVerano = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DiaInicioHorarioVerano = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    HoraInicioHorarioVerano = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    DiferenciaHorariaVerano = table.Column<int>(type: "int", nullable: false),
                    MesInicioHorarioInvierno = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DiaInicioHorarioInvierno = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    HoraInicioHorarioInvierno = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    DiferenciaHorariaInvierno = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_CodigoPostal", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_Colonia",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    CodigoPostal = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_Colonia", x => new { x.Clave, x.CodigoPostal });
                });

            migrationBuilder.CreateTable(
                name: "c_Estado",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_Estado", x => new { x.Clave, x.Pais });
                });

            migrationBuilder.CreateTable(
                name: "c_Localidad",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_Localidad", x => new { x.Clave, x.Estado });
                });

            migrationBuilder.CreateTable(
                name: "c_Municipio",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_Municipio", x => new { x.Clave, x.Estado });
                });

            migrationBuilder.CreateTable(
                name: "c_NumPedimentoAduana",
                schema: "sat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Aduana = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Patente = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Ejercicio = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_NumPedimentoAduana", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "c_Pais",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    FormatoCodigoPostal = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FormatoRegistroIdentidadTributaria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ValidacionRegistroIdentidadTributaria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Agrupaciones = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_Pais", x => x.Clave);
                });

            migrationBuilder.CreateIndex(
                name: "IX_c_NumPedimentoAduana_Aduana_Patente_Ejercicio",
                schema: "sat",
                table: "c_NumPedimentoAduana",
                columns: new[] { "Aduana", "Patente", "Ejercicio" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "c_ClaveProdServ",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_ClaveUnidad",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_CodigoPostal",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_Colonia",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_Estado",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_Localidad",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_Municipio",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_NumPedimentoAduana",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_Pais",
                schema: "sat");
        }
    }
}
