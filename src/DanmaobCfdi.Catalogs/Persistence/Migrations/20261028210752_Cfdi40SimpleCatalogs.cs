using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanmaobCfdi.Catalogs.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Cfdi40SimpleCatalogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "c_Aduana",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_Aduana", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_Exportacion",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_Exportacion", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_Meses",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_Meses", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_MetodoPago",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_MetodoPago", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_MotivoCancelacion",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_MotivoCancelacion", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_ObjetoImp",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_ObjetoImp", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_PatenteAduanal",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_PatenteAduanal", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_Periodicidad",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_Periodicidad", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_TipoCadenaPago",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_TipoCadenaPago", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_TipoFactor",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_TipoFactor", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_TipoRelacion",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_TipoRelacion", x => x.Clave);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "c_Aduana",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_Exportacion",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_Meses",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_MetodoPago",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_MotivoCancelacion",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_ObjetoImp",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_PatenteAduanal",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_Periodicidad",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_TipoCadenaPago",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_TipoFactor",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_TipoRelacion",
                schema: "sat");
        }
    }
}
