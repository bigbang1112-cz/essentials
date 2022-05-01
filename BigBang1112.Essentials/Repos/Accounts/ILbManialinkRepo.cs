using BigBang1112.Models.Db;

namespace BigBang1112.Repos.Accounts;

public interface ILbManialinkRepo : IRepo<ManiaPlanetLbManialinkModel>
{
    Task<IEnumerable<ManiaPlanetLbManialinkModel>> GetMembersAsync(CancellationToken cancellationToken = default);
}
