using BigBang1112.Models.Db;

namespace BigBang1112.Repos.Accounts;

public interface IZoneRepo : IRepo<ZoneModel>
{
    Task<ZoneModel> GetOrAddAsync(string zone, CancellationToken cancellationToken = default);
}
