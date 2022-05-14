using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;

namespace BigBang1112.DiscordBot.Repos;

public interface IReportChannelMessageRepo : IRepo<ReportChannelMessageModel>
{
    Task<IEnumerable<ReportChannelMessageModel>> GetAllByReportGuidAsync(Guid guid, CancellationToken cancellationToken = default);
}
