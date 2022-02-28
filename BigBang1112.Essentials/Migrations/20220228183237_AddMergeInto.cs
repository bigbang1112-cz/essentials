using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BigBang1112.Migrations
{
    public partial class AddMergeInto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MergedIntoId",
                table: "Accounts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_MergedIntoId",
                table: "Accounts",
                column: "MergedIntoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Accounts_MergedIntoId",
                table: "Accounts",
                column: "MergedIntoId",
                principalTable: "Accounts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Accounts_MergedIntoId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_MergedIntoId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "MergedIntoId",
                table: "Accounts");
        }
    }
}
