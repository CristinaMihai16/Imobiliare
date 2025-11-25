using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Imobiliare.Migrations
{
    /// <inheritdoc />
    public partial class Intermediar1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorite_Anunturi_AnunturiID_Anunt",
                table: "Favorite");

            migrationBuilder.DropIndex(
                name: "IX_Favorite_AnunturiID_Anunt",
                table: "Favorite");

            migrationBuilder.DropColumn(
                name: "AnunturiID_Anunt",
                table: "Favorite");

            migrationBuilder.CreateIndex(
                name: "IX_Favorite_ID_Anunt",
                table: "Favorite",
                column: "ID_Anunt");

            migrationBuilder.CreateIndex(
                name: "IX_Conversatii_ID_Anunt",
                table: "Conversatii",
                column: "ID_Anunt");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversatii_Anunturi_ID_Anunt",
                table: "Conversatii",
                column: "ID_Anunt",
                principalTable: "Anunturi",
                principalColumn: "ID_Anunt",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorite_Anunturi_ID_Anunt",
                table: "Favorite",
                column: "ID_Anunt",
                principalTable: "Anunturi",
                principalColumn: "ID_Anunt",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversatii_Anunturi_ID_Anunt",
                table: "Conversatii");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorite_Anunturi_ID_Anunt",
                table: "Favorite");

            migrationBuilder.DropIndex(
                name: "IX_Favorite_ID_Anunt",
                table: "Favorite");

            migrationBuilder.DropIndex(
                name: "IX_Conversatii_ID_Anunt",
                table: "Conversatii");

            migrationBuilder.AddColumn<int>(
                name: "AnunturiID_Anunt",
                table: "Favorite",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favorite_AnunturiID_Anunt",
                table: "Favorite",
                column: "AnunturiID_Anunt");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorite_Anunturi_AnunturiID_Anunt",
                table: "Favorite",
                column: "AnunturiID_Anunt",
                principalTable: "Anunturi",
                principalColumn: "ID_Anunt");
        }
    }
}
