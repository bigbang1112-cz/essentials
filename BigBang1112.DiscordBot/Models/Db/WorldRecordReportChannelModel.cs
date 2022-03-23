using System.ComponentModel.DataAnnotations;

namespace BigBang1112.DiscordBot.Models.Db;

public class WorldRecordReportChannelModel
{
    public int Id { get; set; }

    [Required]
    public virtual DiscordBotJoinedGuildModel JoinedGuild { get; set; } = default!;

    [Required]
    public virtual DiscordBotChannelModel Channel { get; set; } = default!;

    public bool Enabled { get; set; }
}
