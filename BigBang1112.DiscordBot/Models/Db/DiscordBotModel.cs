using System.ComponentModel.DataAnnotations;
using BigBang1112.Models.Db;

namespace BigBang1112.DiscordBot.Models.Db;

public class DiscordBotModel : DbModel
{
    [Required]
    public Guid Guid { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = default!;

    public virtual ICollection<DiscordBotJoinedGuildModel> JoinedGuilds { get; set; } = default!;
}
