using System.ComponentModel.DataAnnotations;

namespace BigBang1112.DiscordBot.Models.Db;

public class FeedbackModel
{
    public int Id { get; set; }

    [Required]
    [Encrypted]
    [StringLength(5000)]
    public string Text { get; set; } = default!;

    [Required]
    public virtual DiscordUserModel User { get; set; } = default!;

    [Required]
    public DateTime WrittenOn { get; set; }

    public bool Responded { get; set; }
}
