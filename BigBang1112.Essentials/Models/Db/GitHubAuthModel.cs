using System.ComponentModel.DataAnnotations;

namespace BigBang1112.Models.Db;

public class GitHubAuthModel
{
    public int Id { get; set; }

    [Required]
    public uint Uid { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string DisplayName { get; set; } = string.Empty;

    [Encrypted]
    [StringLength(255)]
    public string? Email { get; set; }

    public virtual AccountModel? Account { get; set; }
}
