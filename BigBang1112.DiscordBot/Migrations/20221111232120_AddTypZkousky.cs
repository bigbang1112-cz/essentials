using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BigBang1112.DiscordBot.Migrations
{
    public partial class AddTypZkousky : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TypZkousky",
                table: "Predmety",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypZkousky",
                table: "Predmety");
        }
    }
}
