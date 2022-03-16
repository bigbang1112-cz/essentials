using BigBang1112.Models.Db;

namespace BigBang1112.Data;

public interface IAccountsRepo
{
    Task AddAccountAsync(AccountModel account, CancellationToken cancellationToken = default);
    Task AddManiaPlanetAuthAsync(ManiaPlanetAuthModel maniaPlanetAuth, CancellationToken cancellationToken = default);
    AccountModel CreateNewAccount();
    Task<AccountModel?> GetAccountByGuidAsync(Guid guid, CancellationToken cancellationToken = default);
    Task<List<ManiaPlanetLbManialinkModel>> GetLbManialinkMembersAsync(CancellationToken cancellationToken = default);
    Task<ManiaPlanetAuthModel?> GetManiaPlanetAuthByAccessTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<ManiaPlanetAuthModel?> GetManiaPlanetAuthByLoginAsync(string login, CancellationToken cancellationToken = default);
    Task<DiscordAuthModel> GetOrAddDiscordAuthAsync(ulong snowflake, CancellationToken cancellationToken = default);
    Task<GitHubAuthModel> GetOrAddGitHubAuthAsync(uint uid, CancellationToken cancellationToken = default);
    Task<ManiaPlanetAuthModel> GetOrAddManiaPlanetAuthAsync(string login, CancellationToken cancellationToken = default);
    Task<TrackmaniaAuthModel> GetOrAddTrackmaniaAuthAsync(Guid loginGuid, CancellationToken cancellationToken = default);
    Task<TwitterAuthModel> GetOrAddTwitterAuthAsync(ulong userId, CancellationToken cancellationToken = default);
    Task<ZoneModel> GetOrAddZoneAsync(string zone, CancellationToken cancellationToken = default);
    Task<bool> IsAdminAsync(AccountModel account, CancellationToken cancellationToken = default);
    void RemoveAccount(AccountModel account);
    Task<int> SaveAsync(CancellationToken cancellationToken = default);
}
