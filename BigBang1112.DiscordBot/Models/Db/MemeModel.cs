using BigBang1112.Data;
using System.ComponentModel.DataAnnotations;

namespace BigBang1112.DiscordBot.Models.Db;

public class MemeModel
{
    public int Id { get; set; }

    [Required]
    public Guid Guid { get; set; }

    [Required]
    public virtual DiscordBotJoinedGuildModel JoinedGuild { get; set; } = default!;

    [Required]
    [Encrypted]
    public string Content { get; set; } = default!;

    public ulong? AuthorSnowflake { get; set; }

    public DateTime? AddedOn { get; set; }

    [StringLength(255)]
    public string? Attachment { get; set; }
}
