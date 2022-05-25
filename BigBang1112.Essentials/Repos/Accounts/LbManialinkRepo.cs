using BigBang1112.Data;
using BigBang1112.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.Repos.Accounts;

public class LbManialinkRepo : Repo<ManiaPlanetLbManialinkModel>, ILbManialinkRepo
{
    private readonly AccountsContext _context;

    public LbManialinkRepo(AccountsContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ManiaPlanetLbManialinkModel>> GetMembersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LbManialink
            .Include(x => x.Auth)
                .ThenInclude(x => x.Zone)
            .OrderByDescending(x => x.LastVisitedOn)
            .ToListAsync(cancellationToken);
    }
}
