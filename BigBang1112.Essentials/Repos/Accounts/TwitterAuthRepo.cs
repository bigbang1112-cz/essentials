using BigBang1112.Data;
using BigBang1112.Extensions;
using BigBang1112.Models.Db;

namespace BigBang1112.Repos.Accounts;

public class TwitterAuthRepo : Repo<TwitterAuthModel>, ITwitterAuthRepo
{
    private readonly AccountsContext _context;

    public TwitterAuthRepo(AccountsContext context) : base(context)
    {
        _context = context;
    }

    public async Task<TwitterAuthModel> GetOrAddAsync(ulong userId, CancellationToken cancellationToken = default)
    {
        var auth = await _context.TwitterAuth.FirstOrAddAsync(x => x.UserId == userId, () => new TwitterAuthModel
        {
            Account = AccountModel.New(),
            UserId = userId
        }, cancellationToken: cancellationToken);

        auth.Account ??= AccountModel.New();

        return auth;
    }
}
