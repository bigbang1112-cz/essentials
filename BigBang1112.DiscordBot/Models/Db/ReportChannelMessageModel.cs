using BigBang1112.Models.Db;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BigBang1112.DiscordBot.Models.Db;

public class ReportChannelMessageModel : DbModel
{
    public ulong MessageId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime SentOn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ModifiedOn { get; set; }

    [Required]
    public virtual ReportChannelModel Channel { get; set; } = default!;

    public Guid? ReportGuid { get; set; }

    public bool RemovedOfficially { get; set; }
    public bool RemovedByUser { get; set; }
}
