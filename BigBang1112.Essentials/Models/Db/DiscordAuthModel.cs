using System.ComponentModel.DataAnnotations;

namespace BigBang1112.Models.Db;

public class DiscordAuthModel
{
    public int Id { get; set; }

    [Required]
    public ulong Snowflake { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public short Discriminator { get; set; }

    [Required]
    [StringLength(255)]
    public string AvatarHash { get; set; } = string.Empty;

    public virtual AccountModel? Account { get; set; }
}
