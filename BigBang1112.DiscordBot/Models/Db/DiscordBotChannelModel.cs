using System.ComponentModel.DataAnnotations;

namespace BigBang1112.DiscordBot.Models.Db;

public class DiscordBotChannelModel
{
    public int Id { get; set; }

    [Required]
    public ulong Snowflake { get; set; }

    [StringLength(255)]
    public string? Name { get; set; }

    [Required]
    public virtual DiscordBotGuildModel Guild { get; set; } = default!;
}
