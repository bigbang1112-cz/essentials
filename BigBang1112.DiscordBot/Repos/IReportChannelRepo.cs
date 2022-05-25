using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Repos;

public interface IReportChannelRepo : IRepo<ReportChannelModel>
{
    Task AddOrUpdateAsync(Guid discordBotGuid, SocketTextChannel textChannel, string scopeSet, CancellationToken cancellationToken = default);
    Task<ReportChannelModel?> GetByBotAndTextChannelAsync(Guid discordBotGuid, SocketTextChannel textChannel, CancellationToken cancellationToken = default);
    Task<ReportChannelModel?> GetByJoinedGuildOrChannelAsync(DiscordBotJoinedGuildModel joinedGuild, ulong textChannelSnowflake, CancellationToken cancellationToken = default);
    Task<int> CountByJoinedGuildAsync(ulong guildId, CancellationToken cancellationToken = default);
}
