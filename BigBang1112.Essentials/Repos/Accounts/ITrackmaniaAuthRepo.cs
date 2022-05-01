using BigBang1112.Models.Db;

namespace BigBang1112.Repos.Accounts;

public interface ITrackmaniaAuthRepo : IRepo<TrackmaniaAuthModel>
{
    Task<TrackmaniaAuthModel> GetOrAddAsync(Guid loginGuid, CancellationToken cancellationToken = default);
}
