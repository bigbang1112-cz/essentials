using BigBang1112.Repos.Accounts;

namespace BigBang1112.Data;

public interface IAccountsUnitOfWork : IUnitOfWork
{
    IAccountRepo Accounts { get; }
    IAdminRepo Admins { get; }
    IDiscordAuthRepo DiscordAuth { get; }
    IGitHubAuthRepo GitHubAuth { get; }
    ILbManialinkRepo LbManialink { get; }
    IManiaPlanetAuthRepo ManiaPlanetAuth { get; }
    ITrackmaniaAuthRepo TrackmaniaAuth { get; }
    ITwitterAuthRepo TwitterAuth { get; }
    IZoneRepo Zones { get; }
}
