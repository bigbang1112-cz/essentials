using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BigBang1112.Migrations
{
    public partial class GitHubSetNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_GitHubAuth_GitHubId",
                table: "Accounts");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_GitHubAuth_GitHubId",
                table: "Accounts",
                column: "GitHubId",
                principalTable: "GitHubAuth",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_GitHubAuth_GitHubId",
                table: "Accounts");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_GitHubAuth_GitHubId",
                table: "Accounts",
                column: "GitHubId",
                principalTable: "GitHubAuth",
                principalColumn: "Id");
        }
    }
}
