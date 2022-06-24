using BigBang1112.Data;
using BigBang1112.Extensions;
using BigBang1112.Models.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BigBang1112.Repos.Accounts;

public class ManiaPlanetAuthRepo : Repo<ManiaPlanetAuthModel>, IManiaPlanetAuthRepo
{
    private readonly AccountsContext _context;
    private readonly ILogger<AccountsUnitOfWork> _logger;

    public ManiaPlanetAuthRepo(AccountsContext context, ILogger<AccountsUnitOfWork> logger) : base(context)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ManiaPlanetAuthModel?> GetByAccessTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.ManiaPlanetAuth
            .Include(x => x.LbManialink)
            .FirstOrDefaultAsync(x => x.AccessToken == token, cancellationToken);
    }

    public async Task<ManiaPlanetAuthModel?> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.ManiaPlanetAuth
                .FirstOrDefaultAsync(x => x.Login == login, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in ManiaPlanetAuthRepo.GetByLoginAsync - login: {login}", login);

            throw;
        }
    }

    public async Task<ManiaPlanetAuthModel> GetOrAddAsync(string login, CancellationToken cancellationToken = default)
    {
        _ = login ?? throw new ArgumentNullException(nameof(login));

        var auth = await _context.ManiaPlanetAuth.FirstOrAddAsync(x => x.Login == login, () => new ManiaPlanetAuthModel
        {
            Account = AccountModel.New(),
            Login = login
        }, cancellationToken: cancellationToken);

        auth.Account ??= AccountModel.New();

        return auth;
    }
}
