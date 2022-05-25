using BigBang1112.Data;
using BigBang1112.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.Repos.Accounts;

public class AdminRepo : Repo<AdminModel>, IAdminRepo
{
    private readonly AccountsContext _context;

    public AdminRepo(AccountsContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> IsAccountAdminAsync(AccountModel account, CancellationToken cancellationToken = default)
    {
        return await _context.Admins
            .AnyAsync(x => x.Account == account, cancellationToken);
    }
}
