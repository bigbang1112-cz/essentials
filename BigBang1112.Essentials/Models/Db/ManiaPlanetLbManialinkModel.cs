using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BigBang1112.Models.Db;

public class ManiaPlanetLbManialinkModel
{
    public int Id { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime JoinedOn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime LastVisitedOn { get; set; }

    public int Visits { get; set; }
    public bool IsIWRUP { get; set; }

    [StringLength(255)]
    public string SecretKey { get; set; } = default!;

    public virtual ManiaPlanetAuthModel Auth { get; set; } = default!;
}
