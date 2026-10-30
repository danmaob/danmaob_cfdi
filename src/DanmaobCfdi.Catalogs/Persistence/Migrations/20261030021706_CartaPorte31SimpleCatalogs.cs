using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanmaobCfdi.Catalogs.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CartaPorte31SimpleCatalogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "c_ClaveTipoCarga",
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
                    table.PrimaryKey("PK_c_ClaveTipoCarga", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_CondicionesEspeciales",
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
                    table.PrimaryKey("PK_c_CondicionesEspeciales", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_ConfigMaritima",
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
                    table.PrimaryKey("PK_c_ConfigMaritima", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_ContenedorMaritimo",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_ContenedorMaritimo", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_CveTransporte",
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
                    table.PrimaryKey("PK_c_CveTransporte", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_DocumentoAduanero",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_DocumentoAduanero", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_FiguraTransporte",
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
                    table.PrimaryKey("PK_c_FiguraTransporte", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_FormaFarmaceutica",
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
                    table.PrimaryKey("PK_c_FormaFarmaceutica", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_NumAutorizacionNaviero",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_NumAutorizacionNaviero", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_ParteTransporte",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_ParteTransporte", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_RegistroISTMO",
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
                    table.PrimaryKey("PK_c_RegistroISTMO", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_SectorCOFEPRIS",
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
                    table.PrimaryKey("PK_c_SectorCOFEPRIS", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_SubTipoRem",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_SubTipoRem", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_TipoDeTrafico",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_TipoDeTrafico", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_TipoEmbalaje",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_TipoEmbalaje", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_TipoMateria",
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
                    table.PrimaryKey("PK_c_TipoMateria", x => x.Clave);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "c_ClaveTipoCarga",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_CondicionesEspeciales",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_ConfigMaritima",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_ContenedorMaritimo",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_CveTransporte",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_DocumentoAduanero",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_FiguraTransporte",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_FormaFarmaceutica",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_NumAutorizacionNaviero",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_ParteTransporte",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_RegistroISTMO",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_SectorCOFEPRIS",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_SubTipoRem",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_TipoDeTrafico",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_TipoEmbalaje",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_TipoMateria",
                schema: "sat");
        }
    }
}
