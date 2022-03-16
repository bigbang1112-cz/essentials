using System.ComponentModel.DataAnnotations;

namespace BigBang1112.Models.Db;

public class DiscordBotModel
{
    public int Id { get; set; }

    [Required]
    public Guid Guid { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = default!;

    public virtual ICollection<DiscordBotJoinedGuildModel> JoinedGuilds { get; set; } = default!;
}
