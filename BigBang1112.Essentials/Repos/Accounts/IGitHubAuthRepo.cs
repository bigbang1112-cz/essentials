using BigBang1112.Models.Db;

namespace BigBang1112.Repos.Accounts;

public interface IGitHubAuthRepo : IRepo<GitHubAuthModel>
{
    Task<GitHubAuthModel> GetOrAddAsync(uint uid, CancellationToken cancellationToken = default);
}
