using System.ComponentModel.DataAnnotations;
using BigBang1112.Models.Db;

namespace BigBang1112.DiscordBot.Models.Db;

public class ReportChannelModel : DbModel
{
    [Required]
    public virtual DiscordBotJoinedGuildModel JoinedGuild { get; set; } = default!;

    [Required]
    public virtual DiscordBotChannelModel Channel { get; set; } = default!;

    public string? Scope { get; set; }

    public virtual ICollection<ReportChannelMessageModel> Messages { get; set; } = default!;
}
