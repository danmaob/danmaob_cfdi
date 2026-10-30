using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanmaobCfdi.Catalogs.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CartaPorte31OwnStructureCatalogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "c_ClaveProdServCP",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    PalabrasSimilares = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true, collation: "Latin1_General_100_CI_AI"),
                    MaterialPeligroso = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_ClaveProdServCP", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_ClaveUnidadPeso",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    DescripcionUnidad = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Nota = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Simbolo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Bandera = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_ClaveUnidadPeso", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_CodigoTransporteAereo",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Nacionalidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DesignadorOACI = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_CodigoTransporteAereo", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_ConfigAutotransporte",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    NumeroEjes = table.Column<int>(type: "int", nullable: true),
                    NumeroLlantas = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Remolque = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_ConfigAutotransporte", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_Contenedor",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    TipoContenedor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_Contenedor", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_DerechosDePaso",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    Entre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Hasta = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    OtorgaRecibe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Concesionario = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_DerechosDePaso", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_Estaciones",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    ClaveTransporte = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Nacionalidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DesignadorIATA = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LineaFerrea = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_Estaciones", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_FraccionArancelaria",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    UMT = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_FraccionArancelaria", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_MaterialPeligroso",
                schema: "sat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaseODivision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PeligroSecundario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NombreTecnico = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Clave = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_MaterialPeligroso", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "c_RegimenAduanero",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ImpoExpo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_RegimenAduanero", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_TipoCarro",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Contenedor = table.Column<bool>(type: "bit", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_TipoCarro", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_TipoDeServicio",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Contenedor = table.Column<bool>(type: "bit", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_TipoDeServicio", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_TipoEstacion",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    ClaveTransporte = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_TipoEstacion", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_TipoPermiso",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    ClaveTransporte = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_TipoPermiso", x => x.Clave);
                });

            migrationBuilder.CreateIndex(
                name: "IX_c_MaterialPeligroso_Clave",
                schema: "sat",
                table: "c_MaterialPeligroso",
                column: "Clave");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "c_ClaveProdServCP",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_ClaveUnidadPeso",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_CodigoTransporteAereo",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_ConfigAutotransporte",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_Contenedor",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_DerechosDePaso",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_Estaciones",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_FraccionArancelaria",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_MaterialPeligroso",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_RegimenAduanero",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_TipoCarro",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_TipoDeServicio",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_TipoEstacion",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_TipoPermiso",
                schema: "sat");
        }
    }
}
