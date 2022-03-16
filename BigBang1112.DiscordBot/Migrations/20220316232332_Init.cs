using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BigBang1112.DiscordBot.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DiscordBotGuilds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Snowflake = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordBotGuilds", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DiscordBots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordBots", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DiscordBotChannels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Snowflake = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GuildId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordBotChannels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordBotChannels_DiscordBotGuilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "DiscordBotGuilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                    JoinedGuildId = table.Column<int>(type: "int", nullable: false),
                    ChannelId = table.Column<int>(type: "int", nullable: false),
                    Visibility = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordBotCommandVisibilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordBotCommandVisibilities_DiscordBotChannels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "DiscordBotChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscordBotCommandVisibilities_DiscordBotJoinedGuilds_JoinedG~",
                        column: x => x.JoinedGuildId,
                        principalTable: "DiscordBotJoinedGuilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordBotCommandVisibilities_ChannelId",
                table: "DiscordBotCommandVisibilities",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordBotCommandVisibilities_JoinedGuildId",
                table: "DiscordBotCommandVisibilities",
                column: "JoinedGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordBotChannels_GuildId",
                table: "DiscordBotChannels",
                column: "GuildId");

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
                name: "DiscordBotChannels");

            migrationBuilder.DropTable(
                name: "DiscordBotJoinedGuilds");

            migrationBuilder.DropTable(
                name: "DiscordBotGuilds");

            migrationBuilder.DropTable(
                name: "DiscordBots");
        }
    }
}
