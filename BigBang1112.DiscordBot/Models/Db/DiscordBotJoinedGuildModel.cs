using System.ComponentModel.DataAnnotations;
using BigBang1112.Models.Db;

namespace BigBang1112.DiscordBot.Models.Db;

public class DiscordBotJoinedGuildModel : DbModel
{
    [Required]
    public virtual DiscordBotModel Bot { get; set; } = default!;

    [Required]
    public virtual DiscordBotGuildModel Guild { get; set; } = default!;

    public bool CommandVisibility { get; set; }

    public virtual ICollection<DiscordBotCommandVisibilityModel> CommandVisibilities { get; set; } = default!;
}
