using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Imobiliare.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Locatie",
                table: "Anunturi",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Descriere",
                table: "Anunturi",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "ImaginiAnunt",
                columns: table => new
                {
                    ID_ImaginiAnunt = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Imagine = table.Column<byte[]>(type: "bytea", nullable: true),
                    ID_Anunt = table.Column<int>(type: "integer", nullable: false),
                    AnuntID_Anunt = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImaginiAnunt", x => x.ID_ImaginiAnunt);
                    table.ForeignKey(
                        name: "FK_ImaginiAnunt_Anunturi_AnuntID_Anunt",
                        column: x => x.AnuntID_Anunt,
                        principalTable: "Anunturi",
                        principalColumn: "ID_Anunt");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImaginiAnunt_AnuntID_Anunt",
                table: "ImaginiAnunt",
                column: "AnuntID_Anunt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImaginiAnunt");

            migrationBuilder.AlterColumn<string>(
                name: "Locatie",
                table: "Anunturi",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descriere",
                table: "Anunturi",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
