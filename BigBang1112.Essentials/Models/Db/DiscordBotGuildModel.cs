using System.ComponentModel.DataAnnotations;

namespace BigBang1112.Models.Db;

public class DiscordBotGuildModel
{
    public int Id { get; set; }

    [Required]
    public ulong Snowflake { get; set; }

    [StringLength(255)]
    public string? Name { get; set; }

    public virtual ICollection<DiscordBotChannelModel> Channels { get; set; } = default!;
}