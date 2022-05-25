using BigBang1112.Models.Db;

namespace BigBang1112.Repos.Accounts;

public interface ITwitterAuthRepo : IRepo<TwitterAuthModel>
{
    Task<TwitterAuthModel> GetOrAddAsync(ulong userId, CancellationToken cancellationToken = default);
}
