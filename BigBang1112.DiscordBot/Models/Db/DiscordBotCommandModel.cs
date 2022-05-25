using System.ComponentModel.DataAnnotations;
using BigBang1112.Models.Db;

namespace BigBang1112.DiscordBot.Models.Db;

public class DiscordBotCommandModel : DbModel
{
    [Required]
    [StringLength(255)]
    public string CommandName { get; set; } = default!;

    public int Used { get; set; }
    public DateTime LastUsedOn { get; set; }

    [Required]
    public virtual DiscordBotModel Bot { get; set; } = default!;
}
