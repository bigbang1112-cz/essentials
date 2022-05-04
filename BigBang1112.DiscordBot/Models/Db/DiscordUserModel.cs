using System.ComponentModel.DataAnnotations;
using BigBang1112.Models.Db;

namespace BigBang1112.DiscordBot.Models.Db;

public class DiscordUserModel : DbModel
{
    public ulong Snowflake { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public ushort Discriminator { get; set; } = default!;

    [Required]
    public virtual DiscordBotModel Bot { get; set; } = default!;

    [Required]
    public DateTime FirstInteractionOn { get; set; } = default!;

    [Required]
    public DateTime LastInteractionOn { get; set; } = default!;

    public int Interactions { get; set; }

    public bool IsBlocked { get; set; }
}
