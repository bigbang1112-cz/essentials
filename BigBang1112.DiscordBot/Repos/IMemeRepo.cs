using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;

namespace BigBang1112.DiscordBot.Repos;

public interface IMemeRepo : IRepo<MemeModel>
{
    Task<bool> ExistsAsync(string content, CancellationToken cancellationToken = default);
    Task<IEnumerable<MemeModel>> GetFromGuildAsync(DiscordBotJoinedGuildModel joinedGuild, CancellationToken cancellationToken = default);
    Task<MemeModel?> GetLastAsync(DiscordBotJoinedGuildModel joinedGuild, CancellationToken cancellationToken = default);
    Task<MemeModel?> GetRandomAsync(DiscordBotJoinedGuildModel joinedGuild, CancellationToken cancellationToken = default);
}
