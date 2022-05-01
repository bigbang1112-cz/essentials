using System.ComponentModel.DataAnnotations;

namespace BigBang1112.Models.Db;

public class TrackmaniaAuthModel : DbModel
{
    [Required]
    public Guid Login { get; set; }

    [StringLength(255)]
    public string? Nickname { get; set; }

    public virtual AccountModel? Account { get; set; }
}
