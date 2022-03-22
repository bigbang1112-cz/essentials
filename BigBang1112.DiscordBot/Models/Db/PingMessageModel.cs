using System.ComponentModel.DataAnnotations;

namespace BigBang1112.DiscordBot.Models.Db;

public class PingMessageModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(2000)]
    public string Text { get; set; } = default!;

    [Required]
    public virtual DiscordUserModel User { get; set; } = default!;

    [Required]
    public DateTime WrittenOn { get; set; }
}
