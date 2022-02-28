using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BigBang1112.Migrations
{
    public partial class AddMoreSetNulls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_DiscordAuth_DiscordId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_ManiaPlanetAuth_ManiaPlanetId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_TrackmaniaAuth_TrackmaniaId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_TwitterAuth_TwitterId",
                table: "Accounts");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_DiscordAuth_DiscordId",
                table: "Accounts",
                column: "DiscordId",
                principalTable: "DiscordAuth",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_ManiaPlanetAuth_ManiaPlanetId",
                table: "Accounts",
                column: "ManiaPlanetId",
                principalTable: "ManiaPlanetAuth",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_TrackmaniaAuth_TrackmaniaId",
                table: "Accounts",
                column: "TrackmaniaId",
                principalTable: "TrackmaniaAuth",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_TwitterAuth_TwitterId",
                table: "Accounts",
                column: "TwitterId",
                principalTable: "TwitterAuth",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_DiscordAuth_DiscordId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_ManiaPlanetAuth_ManiaPlanetId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_TrackmaniaAuth_TrackmaniaId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_TwitterAuth_TwitterId",
                table: "Accounts");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_DiscordAuth_DiscordId",
                table: "Accounts",
                column: "DiscordId",
                principalTable: "DiscordAuth",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_ManiaPlanetAuth_ManiaPlanetId",
                table: "Accounts",
                column: "ManiaPlanetId",
                principalTable: "ManiaPlanetAuth",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_TrackmaniaAuth_TrackmaniaId",
                table: "Accounts",
                column: "TrackmaniaId",
                principalTable: "TrackmaniaAuth",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_TwitterAuth_TwitterId",
                table: "Accounts",
                column: "TwitterId",
                principalTable: "TwitterAuth",
                principalColumn: "Id");
        }
    }
}
