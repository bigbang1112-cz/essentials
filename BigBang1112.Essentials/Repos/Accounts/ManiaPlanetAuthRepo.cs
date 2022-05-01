using BigBang1112.Data;
using BigBang1112.Extensions;
using BigBang1112.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.Repos.Accounts;

public class ManiaPlanetAuthRepo : Repo<ManiaPlanetAuthModel>, IManiaPlanetAuthRepo
{
    private readonly AccountsContext _context;

    public ManiaPlanetAuthRepo(AccountsContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ManiaPlanetAuthModel?> GetByAccessTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.ManiaPlanetAuth
            .Include(x => x.LbManialink)
            .FirstOrDefaultAsync(x => x.AccessToken == token, cancellationToken);
    }

    public async Task<ManiaPlanetAuthModel?> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        return await _context.ManiaPlanetAuth
            .FirstOrDefaultAsync(x => string.Equals(x.Login, login), cancellationToken);
    }

    public async Task<ManiaPlanetAuthModel> GetOrAddAsync(string login, CancellationToken cancellationToken = default)
    {
        var auth = await _context.ManiaPlanetAuth.FirstOrAddAsync(x => x.Login == login, () => new ManiaPlanetAuthModel
        {
            Account = AccountModel.New(),
            Login = login
        }, cancellationToken: cancellationToken);

        auth.Account ??= AccountModel.New();

        return auth;
    }
}
