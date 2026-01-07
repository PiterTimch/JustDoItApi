using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JustDoItApi.Migrations
{
    /// <inheritdoc />
    public partial class adduserzadacharel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "tbl_zadacha",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_zadacha_UserId",
                table: "tbl_zadacha",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_zadacha_AspNetUsers_UserId",
                table: "tbl_zadacha",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_zadacha_AspNetUsers_UserId",
                table: "tbl_zadacha");

            migrationBuilder.DropIndex(
                name: "IX_tbl_zadacha_UserId",
                table: "tbl_zadacha");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "tbl_zadacha");
        }
    }
}
