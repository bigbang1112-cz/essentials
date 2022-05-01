using BigBang1112.Data;
using BigBang1112.Extensions;
using BigBang1112.Models.Db;

namespace BigBang1112.Repos.Accounts;

public class TrackmaniaAuthRepo : Repo<TrackmaniaAuthModel>, ITrackmaniaAuthRepo
{
    private readonly AccountsContext _context;

    public TrackmaniaAuthRepo(AccountsContext context) : base(context)
    {
        _context = context;
    }

    public async Task<TrackmaniaAuthModel> GetOrAddAsync(Guid loginGuid, CancellationToken cancellationToken = default)
    {
        var auth = await _context.TrackmaniaAuth.FirstOrAddAsync(x => x.Login == loginGuid, () => new TrackmaniaAuthModel
        {
            Account = AccountModel.New(),
            Login = loginGuid
        }, cancellationToken: cancellationToken);

        auth.Account ??= AccountModel.New();

        return auth;
    }
}
