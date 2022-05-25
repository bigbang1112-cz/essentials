using BigBang1112.Data;
using BigBang1112.Extensions;
using BigBang1112.Models.Db;

namespace BigBang1112.Repos.Accounts;

// create implementation
public class GitHubAuthRepo : Repo<GitHubAuthModel>, IGitHubAuthRepo
{
    private readonly AccountsContext _context;

    public GitHubAuthRepo(AccountsContext context) : base(context)
    {
        _context = context;
    }

    public async Task<GitHubAuthModel> GetOrAddAsync(uint uid, CancellationToken cancellationToken = default)
    {
        var auth = await _context.GitHubAuth.FirstOrAddAsync(x => x.Uid == uid, () => new GitHubAuthModel
        {
            Account = AccountModel.New(),
            Uid = uid
        }, cancellationToken: cancellationToken);

        auth.Account ??= AccountModel.New();

        return auth;
    }
}
