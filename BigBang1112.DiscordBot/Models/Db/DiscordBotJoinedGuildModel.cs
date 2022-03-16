using System.ComponentModel.DataAnnotations;

namespace BigBang1112.DiscordBot.Models.Db;

public class DiscordBotJoinedGuildModel
{
    public int Id { get; set; }

    [Required]
    public virtual DiscordBotModel Bot { get; set; } = default!;

    [Required]
    public virtual DiscordBotGuildModel Guild { get; set; } = default!;

    public bool CommandVisibility { get; set; }

    public virtual ICollection<DiscordBotCommandVisibilityModel> CommandVisibilities { get; set; } = default!;
}
