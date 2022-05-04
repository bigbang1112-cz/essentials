using System.ComponentModel.DataAnnotations;
using BigBang1112.Models.Db;

namespace BigBang1112.DiscordBot.Models.Db;

public class DiscordBotGuildModel : DbModel
{
    [Required]
    public ulong Snowflake { get; set; }

    [StringLength(255)]
    public string? Name { get; set; }

    public virtual ICollection<DiscordBotChannelModel> Channels { get; set; } = default!;
}
