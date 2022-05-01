using BigBang1112.Repos.Accounts;
using Microsoft.Extensions.Logging;

namespace BigBang1112.Data;

public class AccountsUnitOfWork : UnitOfWork, IAccountsUnitOfWork
{
    public IAccountRepo Accounts { get; }
    public IAdminRepo Admins { get; }
    public IDiscordAuthRepo DiscordAuth { get; }
    public IGitHubAuthRepo GitHubAuth { get; }
    public IManiaPlanetAuthRepo ManiaPlanetAuth { get; }
    public ITrackmaniaAuthRepo TrackmaniaAuth { get; }
    public ITwitterAuthRepo TwitterAuth { get; }
    public IZoneRepo Zones { get; }
    public ILbManialinkRepo LbManialink { get; }

    public AccountsUnitOfWork(AccountsContext context, ILogger<AccountsUnitOfWork> logger) : base(context, logger)
    {
        Accounts = new AccountRepo(context);
        Admins = new AdminRepo(context);
        DiscordAuth = new DiscordAuthRepo(context);
        GitHubAuth = new GitHubAuthRepo(context);
        ManiaPlanetAuth = new ManiaPlanetAuthRepo(context);
        TrackmaniaAuth = new TrackmaniaAuthRepo(context);
        TwitterAuth = new TwitterAuthRepo(context);
        Zones = new ZoneRepo(context);
        LbManialink = new LbManialinkRepo(context);
    }
}
