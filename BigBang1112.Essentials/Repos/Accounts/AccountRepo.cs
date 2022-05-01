using BigBang1112.Data;
using BigBang1112.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.Repos.Accounts;

public class AccountRepo : Repo<AccountModel>, IAccountRepo
{
    private readonly AccountsContext _context;

    public AccountRepo(AccountsContext context) : base(context)
    {
        _context = context;
    }

    public async Task<AccountModel?> GetByGuidAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(x => x.Guid == guid, cancellationToken);
    }
}
