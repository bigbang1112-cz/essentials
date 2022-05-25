using BigBang1112.Models.Db;

namespace BigBang1112.Repos.Accounts;

public interface IManiaPlanetAuthRepo : IRepo<ManiaPlanetAuthModel>
{
    Task<ManiaPlanetAuthModel?> GetByAccessTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<ManiaPlanetAuthModel?> GetByLoginAsync(string login, CancellationToken cancellationToken = default);
    Task<ManiaPlanetAuthModel> GetOrAddAsync(string login, CancellationToken cancellationToken = default);
}
