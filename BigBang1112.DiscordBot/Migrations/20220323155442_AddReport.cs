using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BigBang1112.DiscordBot.Migrations
{
    public partial class AddReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorldRecordReportChannels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    JoinedGuildId = table.Column<int>(type: "int", nullable: false),
                    ChannelId = table.Column<int>(type: "int", nullable: false),
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorldRecordReportChannels");
        }
    }
}
