using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanmaobCfdi.Catalogs.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSatSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sat");

            migrationBuilder.CreateTable(
                name: "CatalogoVersion",
                schema: "sat",
                columns: table => new
                {
                    Catalogo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VersionSat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RevisionSat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FechaPublicacionSat = table.Column<DateOnly>(type: "date", nullable: true),
                    VersionComponente = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaAplicacionUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogoVersion", x => x.Catalogo);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogoVersion",
                schema: "sat");
        }
    }
}
