using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BigBang1112.DiscordBot.Migrations
{
    public partial class AddReportChannel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorldRecordReportChannels");

            migrationBuilder.CreateTable(
                name: "ReportChannels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    JoinedGuildId = table.Column<int>(type: "int", nullable: false),
                    ChannelId = table.Column<int>(type: "int", nullable: false),
                    Scope = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportChannels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportChannels_DiscordBotChannels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "DiscordBotChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportChannels_DiscordBotJoinedGuilds_JoinedGuildId",
                        column: x => x.JoinedGuildId,
                        principalTable: "DiscordBotJoinedGuilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ReportChannels_ChannelId",
                table: "ReportChannels",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportChannels_JoinedGuildId",
                table: "ReportChannels",
                column: "JoinedGuildId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportChannels");

            migrationBuilder.CreateTable(
                name: "WorldRecordReportChannels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ChannelId = table.Column<int>(type: "int", nullable: false),
                    JoinedGuildId = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldRecordReportChannels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorldRecordReportChannels_DiscordBotChannels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "DiscordBotChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorldRecordReportChannels_DiscordBotJoinedGuilds_JoinedGuild~",
                        column: x => x.JoinedGuildId,
                        principalTable: "DiscordBotJoinedGuilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_WorldRecordReportChannels_ChannelId",
                table: "WorldRecordReportChannels",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_WorldRecordReportChannels_JoinedGuildId",
                table: "WorldRecordReportChannels",
                column: "JoinedGuildId");
        }
    }
}
