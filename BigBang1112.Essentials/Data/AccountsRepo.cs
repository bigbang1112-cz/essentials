using BigBang1112.Extensions;
using BigBang1112.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.Data;

public class AccountsRepo : IAccountsRepo
{
    private readonly AccountsContext _db;

    public AccountsRepo(AccountsContext db)
    {
        _db = db;
    }

    public async Task<ManiaPlanetAuthModel?> GetManiaPlanetAuthByAccessTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _db.ManiaPlanetAuth
            .Include(x => x.LbManialink)
            .FirstOrDefaultAsync(x => x.AccessToken == token, cancellationToken);
    }

    public async Task<ManiaPlanetAuthModel?> GetManiaPlanetAuthByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        return await _db.ManiaPlanetAuth.FirstOrDefaultAsync(x => x.Login == login, cancellationToken);
    }

    public async Task AddManiaPlanetAuthAsync(ManiaPlanetAuthModel maniaPlanetAuth, CancellationToken cancellationToken = default)
    {
        await _db.ManiaPlanetAuth.AddAsync(maniaPlanetAuth, cancellationToken);
    }

    public async Task AddAccountAsync(AccountModel account, CancellationToken cancellationToken = default)
    {
        await _db.Accounts.AddAsync(account, cancellationToken);
    }

    public async Task<ZoneModel> GetOrAddZoneAsync(string zone, CancellationToken cancellationToken = default)
    {
        return await _db.Zones.FirstOrAddAsync(x => x.Name == zone, () => new ZoneModel
        {
            Name = zone
        }, cancellationToken);
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        return await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ManiaPlanetLbManialinkModel>> GetLbManialinkMembersAsync(CancellationToken cancellationToken = default)
    {
        return await _db.LbManialink
            .Include(x => x.Auth)
                .ThenInclude(x => x.Zone)
            .OrderByDescending(x => x.LastVisitedOn)
            .ToListAsync(cancellationToken);
    }

    public AccountModel CreateNewAccount() => new()
    {
        Guid = Guid.NewGuid(),
        CreatedOn = DateTime.UtcNow,
        LastSeenOn = DateTime.UtcNow
    };

    public async Task<TrackmaniaAuthModel> GetOrAddTrackmaniaAuthAsync(Guid loginGuid, CancellationToken cancellationToken = default)
    {
        var auth = await _db.TrackmaniaAuth.FirstOrAddAsync(x => x.Login == loginGuid, () => new TrackmaniaAuthModel
        {
            Account = CreateNewAccount(),
            Login = loginGuid
        }, cancellationToken: cancellationToken);

        auth.Account ??= CreateNewAccount();

        return auth;
    }

    public async Task<ManiaPlanetAuthModel> GetOrAddManiaPlanetAuthAsync(string login, CancellationToken cancellationToken = default)
    {
        var auth = await _db.ManiaPlanetAuth.FirstOrAddAsync(x => x.Login == login, () => new ManiaPlanetAuthModel
        {
            Account = CreateNewAccount(),
            Login = login
        }, cancellationToken);

        auth.Account ??= CreateNewAccount();

        return auth;
    }

    public async Task<DiscordAuthModel> GetOrAddDiscordAuthAsync(ulong snowflake, CancellationToken cancellationToken = default)
    {
        var auth = await _db.DiscordAuth.FirstOrAddAsync(x => x.Snowflake == snowflake, () => new DiscordAuthModel
        {
            Account = CreateNewAccount(),
            Snowflake = snowflake
        }, cancellationToken);

        auth.Account ??= CreateNewAccount();

        return auth;
    }

    public async Task<GitHubAuthModel> GetOrAddGitHubAuthAsync(uint uid, CancellationToken cancellationToken = default)
    {
        var auth = await _db.GitHubAuth.FirstOrAddAsync(x => x.Uid == uid, () => new GitHubAuthModel
        {
            Account = CreateNewAccount(),
            Uid = uid
        }, cancellationToken);

        auth.Account ??= CreateNewAccount();

        return auth;
    }

    public async Task<TwitterAuthModel> GetOrAddTwitterAuthAsync(ulong userId, CancellationToken cancellationToken = default)
    {
        var auth = await _db.TwitterAuth.FirstOrAddAsync(x => x.UserId == userId, () => new TwitterAuthModel
        {
            Account = CreateNewAccount(),
            UserId = userId
        }, cancellationToken);

        auth.Account ??= CreateNewAccount();

        return auth;
    }

    public async Task<bool> IsAdminAsync(AccountModel account, CancellationToken cancellationToken = default)
    {
        return await _db.Admins.AnyAsync(x => x.Account == account, cancellationToken);
    }

    public void RemoveAccount(AccountModel account)
    {
        _ = _db.Accounts.Remove(account);
    }

    public async Task<AccountModel?> GetAccountByGuid(Guid guid, CancellationToken cancellationToken = default)
    {
        return await _db.Accounts.FirstOrDefaultAsync(x => x.Guid == guid, cancellationToken);
    }
}
