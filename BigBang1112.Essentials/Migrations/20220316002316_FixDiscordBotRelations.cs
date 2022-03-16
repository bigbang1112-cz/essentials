using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BigBang1112.Migrations
{
    public partial class FixDiscordBotRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscordBotCommandVisibilities_DiscordBotGuilds_DiscordBotGui~",
                table: "DiscordBotCommandVisibilities");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscordBotCommandVisibilities_DiscordBotJoinedGuilds_Discord~",
                table: "DiscordBotCommandVisibilities");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscordBotCommandVisibilities_DiscordBots_BotId",
                table: "DiscordBotCommandVisibilities");

            migrationBuilder.DropIndex(
                name: "IX_DiscordBotCommandVisibilities_DiscordBotGuildModelId",
                table: "DiscordBotCommandVisibilities");

            migrationBuilder.DropIndex(
                name: "IX_DiscordBotCommandVisibilities_DiscordBotJoinedGuildModelId",
                table: "DiscordBotCommandVisibilities");

            migrationBuilder.DropColumn(
                name: "DiscordBotGuildModelId",
                table: "DiscordBotCommandVisibilities");

            migrationBuilder.DropColumn(
                name: "DiscordBotJoinedGuildModelId",
                table: "DiscordBotCommandVisibilities");

            migrationBuilder.RenameColumn(
                name: "BotId",
                table: "DiscordBotCommandVisibilities",
                newName: "JoinedGuildId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscordBotCommandVisibilities_BotId",
                table: "DiscordBotCommandVisibilities",
                newName: "IX_DiscordBotCommandVisibilities_JoinedGuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscordBotCommandVisibilities_DiscordBotJoinedGuilds_JoinedG~",
                table: "DiscordBotCommandVisibilities",
                column: "JoinedGuildId",
                principalTable: "DiscordBotJoinedGuilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscordBotCommandVisibilities_DiscordBotJoinedGuilds_JoinedG~",
                table: "DiscordBotCommandVisibilities");

            migrationBuilder.RenameColumn(
                name: "JoinedGuildId",
                table: "DiscordBotCommandVisibilities",
                newName: "BotId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscordBotCommandVisibilities_JoinedGuildId",
                table: "DiscordBotCommandVisibilities",
                newName: "IX_DiscordBotCommandVisibilities_BotId");

            migrationBuilder.AddColumn<int>(
                name: "DiscordBotGuildModelId",
                table: "DiscordBotCommandVisibilities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiscordBotJoinedGuildModelId",
                table: "DiscordBotCommandVisibilities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscordBotCommandVisibilities_DiscordBotGuildModelId",
                table: "DiscordBotCommandVisibilities",
                column: "DiscordBotGuildModelId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordBotCommandVisibilities_DiscordBotJoinedGuildModelId",
                table: "DiscordBotCommandVisibilities",
                column: "DiscordBotJoinedGuildModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscordBotCommandVisibilities_DiscordBotGuilds_DiscordBotGui~",
                table: "DiscordBotCommandVisibilities",
                column: "DiscordBotGuildModelId",
                principalTable: "DiscordBotGuilds",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscordBotCommandVisibilities_DiscordBotJoinedGuilds_Discord~",
                table: "DiscordBotCommandVisibilities",
                column: "DiscordBotJoinedGuildModelId",
                principalTable: "DiscordBotJoinedGuilds",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscordBotCommandVisibilities_DiscordBots_BotId",
                table: "DiscordBotCommandVisibilities",
                column: "BotId",
                principalTable: "DiscordBots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
