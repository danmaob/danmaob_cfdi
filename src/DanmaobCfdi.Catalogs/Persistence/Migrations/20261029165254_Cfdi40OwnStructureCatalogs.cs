using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanmaobCfdi.Catalogs.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Cfdi40OwnStructureCatalogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "c_FormaPago",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Bancarizado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumeroOperacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RfcEmisorCuentaOrdenante = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CuentaOrdenante = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PatronCuentaOrdenante = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RfcEmisorCuentaBeneficiario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CuentaBeneficiario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PatronCuentaBeneficiaria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoCadenaPago = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NombreBancoEmisorCuentaOrdenante = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_FormaPago", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_Impuesto",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Retencion = table.Column<bool>(type: "bit", nullable: false),
                    Traslado = table.Column<bool>(type: "bit", nullable: false),
                    LocalOFederal = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_Impuesto", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_Moneda",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Decimales = table.Column<int>(type: "int", nullable: false),
                    PorcentajeVariacion = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_Moneda", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_RegimenFiscal",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    AplicaPersonaFisica = table.Column<bool>(type: "bit", nullable: false),
                    AplicaPersonaMoral = table.Column<bool>(type: "bit", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_RegimenFiscal", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_TasaOCuota",
                schema: "sat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ValorMinimo = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    ValorMaximo = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    Impuesto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Factor = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Traslado = table.Column<bool>(type: "bit", nullable: false),
                    Retencion = table.Column<bool>(type: "bit", nullable: false),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_TasaOCuota", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "c_TipoDeComprobante",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    ValorMaximo = table.Column<decimal>(type: "decimal(24,6)", precision: 24, scale: 6, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_TipoDeComprobante", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_UsoCFDI",
                schema: "sat",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    AplicaPersonaFisica = table.Column<bool>(type: "bit", nullable: false),
                    AplicaPersonaMoral = table.Column<bool>(type: "bit", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, collation: "Latin1_General_100_CI_AI"),
                    FechaInicioVigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFinVigencia = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_UsoCFDI", x => x.Clave);
                });

            migrationBuilder.CreateTable(
                name: "c_UsoCFDI_RegimenFiscalReceptor",
                schema: "sat",
                columns: table => new
                {
                    UsoCFDI = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    RegimenFiscal = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_UsoCFDI_RegimenFiscalReceptor", x => new { x.UsoCFDI, x.RegimenFiscal });
                    table.ForeignKey(
                        name: "FK_c_UsoCFDI_RegimenFiscalReceptor_c_UsoCFDI_UsoCFDI",
                        column: x => x.UsoCFDI,
                        principalSchema: "sat",
                        principalTable: "c_UsoCFDI",
                        principalColumn: "Clave",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_c_TasaOCuota_Tipo_ValorMinimo_ValorMaximo_Impuesto_Factor",
                schema: "sat",
                table: "c_TasaOCuota",
                columns: new[] { "Tipo", "ValorMinimo", "ValorMaximo", "Impuesto", "Factor" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "c_FormaPago",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_Impuesto",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_Moneda",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_RegimenFiscal",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_TasaOCuota",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_TipoDeComprobante",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_UsoCFDI_RegimenFiscalReceptor",
                schema: "sat");

            migrationBuilder.DropTable(
                name: "c_UsoCFDI",
                schema: "sat");
        }
    }
}
