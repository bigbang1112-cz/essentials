using System.ComponentModel.DataAnnotations;

namespace BigBang1112.Models.Db;

public class ZoneModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(1024)]
    public string Name { get; set; } = default!;

    public bool IsTMUF { get; set; }
    public bool IsMP { get; set; }
    public bool IsTM2020 { get; set; }
}
