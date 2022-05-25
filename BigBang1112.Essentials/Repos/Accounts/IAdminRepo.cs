using BigBang1112.Models.Db;

namespace BigBang1112.Repos.Accounts;

public interface IAdminRepo : IRepo<AdminModel>
{
    Task<bool> IsAccountAdminAsync(AccountModel account, CancellationToken cancellationToken = default);
}
