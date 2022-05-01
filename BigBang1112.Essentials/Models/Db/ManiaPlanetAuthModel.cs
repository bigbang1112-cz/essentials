using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BigBang1112.Models.Db;

public class ManiaPlanetAuthModel : DbModel
{
    [Required]
    [StringLength(255)]
    public string Login { get; set; } = default!;

    [Required]
    [StringLength(255)]
    public string Nickname { get; set; } = default!;

    [Required]
    public virtual ZoneModel Zone { get; set; } = default!;

    [Encrypted]
    [StringLength(1024)]
    public string AccessToken { get; set; } = default!;

    [Encrypted]
    [StringLength(1024)]
    public string RefreshToken { get; set; } = default!;

    [Column(TypeName = "datetime")]
    public DateTime? RequestedOn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ExpiresOn { get; set; }

    public virtual ManiaPlanetLbManialinkModel LbManialink { get; set; } = default!;
    public int? LbManialinkId { get; set; }

    public virtual AccountModel Account { get; set; } = default!;
}
