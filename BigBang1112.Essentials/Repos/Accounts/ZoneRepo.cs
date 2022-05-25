using BigBang1112.Data;
using BigBang1112.Extensions;
using BigBang1112.Models.Db;

namespace BigBang1112.Repos.Accounts;

public class ZoneRepo : Repo<ZoneModel>, IZoneRepo
{
    private readonly AccountsContext _context;

    public ZoneRepo(AccountsContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ZoneModel> GetOrAddAsync(string zone, CancellationToken cancellationToken = default)
    {
        return await _context.Zones.FirstOrAddAsync(x => x.Name == zone, () => new ZoneModel
        {
            Name = zone
        }, cancellationToken: cancellationToken);
    }
}
