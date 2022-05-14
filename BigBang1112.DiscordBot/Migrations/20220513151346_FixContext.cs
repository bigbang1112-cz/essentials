using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BigBang1112.DiscordBot.Migrations
{
    public partial class FixContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportChannelMessageModel_ReportChannels_ChannelId",
                table: "ReportChannelMessageModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportChannelMessageModel",
                table: "ReportChannelMessageModel");

            migrationBuilder.RenameTable(
                name: "ReportChannelMessageModel",
                newName: "ReportChannelMessages");

            migrationBuilder.RenameIndex(
                name: "IX_ReportChannelMessageModel_ChannelId",
                table: "ReportChannelMessages",
                newName: "IX_ReportChannelMessages_ChannelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportChannelMessages",
                table: "ReportChannelMessages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportChannelMessages_ReportChannels_ChannelId",
                table: "ReportChannelMessages",
                column: "ChannelId",
                principalTable: "ReportChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportChannelMessages_ReportChannels_ChannelId",
                table: "ReportChannelMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportChannelMessages",
                table: "ReportChannelMessages");

            migrationBuilder.RenameTable(
                name: "ReportChannelMessages",
                newName: "ReportChannelMessageModel");

            migrationBuilder.RenameIndex(
                name: "IX_ReportChannelMessages_ChannelId",
                table: "ReportChannelMessageModel",
                newName: "IX_ReportChannelMessageModel_ChannelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportChannelMessageModel",
                table: "ReportChannelMessageModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportChannelMessageModel_ReportChannels_ChannelId",
                table: "ReportChannelMessageModel",
                column: "ChannelId",
                principalTable: "ReportChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
