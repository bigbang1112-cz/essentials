using System.ComponentModel.DataAnnotations;
using SoftFluent.ComponentModel.DataAnnotations;
using BigBang1112.Models.Db;

namespace BigBang1112.DiscordBot.Models.Db;

public class FeedbackModel : DbModel
{
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
