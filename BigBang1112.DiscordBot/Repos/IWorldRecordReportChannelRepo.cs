using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Repos;

public interface IWorldRecordReportChannelRepo : IRepo<WorldRecordReportChannelModel>
{
    Task AddOrUpdateAsync(Guid discordBotGuid, SocketTextChannel textChannel, bool set, CancellationToken cancellationToken = default);
    Task<WorldRecordReportChannelModel?> GetByBotAndTextChannelAsync(Guid discordBotGuid, SocketTextChannel textChannel, CancellationToken cancellationToken = default);
    Task<WorldRecordReportChannelModel?> GetByJoinedGuildOrChannelAsync(DiscordBotJoinedGuildModel joinedGuild, ulong textChannelSnowflake, CancellationToken cancellationToken = default);
}
