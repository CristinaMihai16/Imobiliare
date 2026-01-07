using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imobiliare.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAnunturiTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Oras",
                table: "Anunturi",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipProprietate",
                table: "Anunturi",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tranzactie",
                table: "Anunturi",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Oras",
                table: "Anunturi");

            migrationBuilder.DropColumn(
                name: "TipProprietate",
                table: "Anunturi");

            migrationBuilder.DropColumn(
                name: "Tranzactie",
                table: "Anunturi");
        }
    }
}
