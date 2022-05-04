using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Repos;

public interface IDiscordBotCommandVisibilityRepo : IRepo<DiscordBotCommandVisibilityModel>
{
    Task AddOrUpdateAsync(Guid discordBotGuid, SocketTextChannel textChannel, bool set, CancellationToken cancellationToken = default);
    Task<DiscordBotCommandVisibilityModel?> GetByBotAndTextChannelAsync(Guid discordBotGuid, SocketTextChannel textChannel, CancellationToken cancellationToken = default);
    Task<DiscordBotCommandVisibilityModel?> GetByJoinedGuildAndChannelAsync(DiscordBotJoinedGuildModel joinedGuild, ulong channelSnowflake, CancellationToken cancellationToken = default);
}
