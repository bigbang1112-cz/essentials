using System.ComponentModel.DataAnnotations;
using BigBang1112.Models.Db;

namespace BigBang1112.DiscordBot.Models.Db;

public class DiscordBotChannelModel : DbModel
{
    [Required]
    public ulong Snowflake { get; set; }

    [StringLength(255)]
    public string? Name { get; set; }

    [Required]
    public virtual DiscordBotGuildModel Guild { get; set; } = default!;
}
