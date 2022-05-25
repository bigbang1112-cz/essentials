using System.ComponentModel.DataAnnotations;

namespace BigBang1112.Models.Db;

public class AccountModel : DbModel
{
    [Required]
    public Guid Guid { get; set; }

    public virtual ManiaPlanetAuthModel? ManiaPlanet { get; set; }
    public int? ManiaPlanetId { get; set; }

    public virtual TrackmaniaAuthModel? Trackmania { get; set; }
    public int? TrackmaniaId { get; set; }

    public virtual DiscordAuthModel? Discord { get; set; }
    public int? DiscordId { get; set; }

    public virtual GitHubAuthModel? GitHub { get; set; }
    public int? GitHubId { get; set; }

    public virtual TwitterAuthModel? Twitter { get; set; }
    public int? TwitterId { get; set; }

    public virtual AdminModel? Admin { get; set; }

    public virtual AccountModel? MergedInto { get; set; }
    public int? MergedIntoId { get; set; }

    [Required]
    public DateTime CreatedOn { get; set; }

    [Required]
    public DateTime LastSeenOn { get; set; }

    public bool IsAdmin => Admin is not null;

    public AccountModel()
    {
        ManiaPlanet = null!;
    }

    public override string ToString()
    {
        return Guid.ToString();
    }

    public static AccountModel New() => new()
    {
        Guid = Guid.NewGuid(),
        CreatedOn = DateTime.UtcNow,
        LastSeenOn = DateTime.UtcNow
    };
}
