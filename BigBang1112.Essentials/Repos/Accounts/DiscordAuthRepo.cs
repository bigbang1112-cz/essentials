using BigBang1112.Data;
using BigBang1112.Extensions;
using BigBang1112.Models.Db;

namespace BigBang1112.Repos.Accounts;

public class DiscordAuthRepo : Repo<DiscordAuthModel>, IDiscordAuthRepo
{
    private readonly AccountsContext _context;

    public DiscordAuthRepo(AccountsContext context) : base(context)
    {
        _context = context;
    }

    public async Task<DiscordAuthModel> GetOrAddAsync(ulong snowflake, CancellationToken cancellationToken = default)
    {
        var auth = await _context.DiscordAuth.FirstOrAddAsync(x => x.Snowflake == snowflake, () => new DiscordAuthModel
        {
            Account = AccountModel.New(),
            Snowflake = snowflake
        }, cancellationToken: cancellationToken);

        auth.Account ??= AccountModel.New();

        return auth;
    }
}
