using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BigBang1112.Migrations
{
    public partial class UpdateDiscordBotStruct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscordBotGuilds_DiscordBots_BotId",
                table: "DiscordBotGuilds");

            migrationBuilder.DropIndex(
                name: "IX_DiscordBotGuilds_BotId",
                table: "DiscordBotGuilds");

            migrationBuilder.DropColumn(
                name: "CommandVisibility",
                table: "DiscordBotChannels");

            migrationBuilder.DropColumn(
                name: "BotId",
                table: "DiscordBotGuilds");

            migrationBuilder.CreateTable(
                name: "DiscordBotJoinedGuilds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BotId = table.Column<int>(type: "int", nullable: false),
                    GuildId = table.Column<int>(type: "int", nullable: false),
                    CommandVisibility = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordBotJoinedGuilds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordBotJoinedGuilds_DiscordBotGuilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "DiscordBotGuilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscordBotJoinedGuilds_DiscordBots_BotId",
                        column: x => x.BotId,
                        principalTable: "DiscordBots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DiscordBotCommandVisibilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BotId = table.Column<int>(type: "int", nullable: false),
                    ChannelId = table.Column<int>(type: "int", nullable: false),
                    Visibility = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DiscordBotGuildModelId = table.Column<int>(type: "int", nullable: true),
                    DiscordBotJoinedGuildModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordBotCommandVisibilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordBotCommandVisibilities_DiscordBotGuilds_DiscordBotGui~",
                        column: x => x.DiscordBotGuildModelId,
                        principalTable: "DiscordBotGuilds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DiscordBotCommandVisibilities_DiscordBotChannels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "DiscordBotChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscordBotCommandVisibilities_DiscordBotJoinedGuilds_Discord~",
                        column: x => x.DiscordBotJoinedGuildModelId,
                        principalTable: "DiscordBotJoinedGuilds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DiscordBotCommandVisibilities_DiscordBots_BotId",
                        column: x => x.BotId,
                        principalTable: "DiscordBots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordBotCommandVisibilities_BotId",
                table: "DiscordBotCommandVisibilities",
                column: "BotId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordBotCommandVisibilities_DiscordBotGuildModelId",
                table: "DiscordBotCommandVisibilities",
                column: "DiscordBotGuildModelId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordBotCommandVisibilities_DiscordBotJoinedGuildModelId",
                table: "DiscordBotCommandVisibilities",
                column: "DiscordBotJoinedGuildModelId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordBotCommandVisibilities_ChannelId",
                table: "DiscordBotCommandVisibilities",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordBotJoinedGuilds_BotId",
                table: "DiscordBotJoinedGuilds",
                column: "BotId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordBotJoinedGuilds_GuildId",
                table: "DiscordBotJoinedGuilds",
                column: "GuildId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscordBotCommandVisibilities");

            migrationBuilder.DropTable(
                name: "DiscordBotJoinedGuilds");

            migrationBuilder.AddColumn<bool>(
                name: "CommandVisibility",
                table: "DiscordBotChannels",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "BotId",
                table: "DiscordBotGuilds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DiscordBotGuilds_BotId",
                table: "DiscordBotGuilds",
                column: "BotId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscordBotGuilds_DiscordBots_BotId",
                table: "DiscordBotGuilds",
                column: "BotId",
                principalTable: "DiscordBots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
