using BigBang1112.Models.Db;
using System.ComponentModel.DataAnnotations;

namespace BigBang1112.UniReminder.Models;

public class PredmetModel : DbModel
{
    [Required]
    public Guid Guid { get; set; }

    [Required]
    [StringLength(255)]
    public string Pracoviste { get; set; } = default!;
    
    [Required]
    [StringLength(255)]
    public string Predmet { get; set; } = default!;

    [Required]
    [StringLength(255)]
    public string Nazev { get; set; } = default!;
    
    [Required]
    public bool ZS { get; set; }

    [Required]
    public bool LS { get; set; }

    [Required]
    public byte Kredity { get; set; }

    [Required]
    [StringLength(255)]
    public string TypZkousky { get; set; } = default!;
    
    public string? Pozadavky { get; set; }
}
