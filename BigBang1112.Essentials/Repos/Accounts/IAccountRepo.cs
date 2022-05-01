using BigBang1112.Models.Db;

namespace BigBang1112.Repos.Accounts;

public interface IAccountRepo : IRepo<AccountModel>
{
    Task<AccountModel?> GetByGuidAsync(Guid guid, CancellationToken cancellationToken = default);
}
