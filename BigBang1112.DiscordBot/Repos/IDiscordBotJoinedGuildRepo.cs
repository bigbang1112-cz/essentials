using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Repos;

public interface IDiscordBotJoinedGuildRepo : IRepo<DiscordBotJoinedGuildModel>
{
    Task<DiscordBotJoinedGuildModel?> GetByBotAndGuildAsync(DiscordBotModel discordBotModel, SocketGuild guild, CancellationToken cancellationToken = default);
    Task<DiscordBotJoinedGuildModel?> GetByBotAndTextChannelAsync(Guid discordBotGuid, SocketTextChannel textChannel, CancellationToken cancellationToken = default);
    Task<DiscordBotJoinedGuildModel> GetOrAddAsync(DiscordBotModel discordBot, SocketGuild guild, CancellationToken cancellationToken = default);
    Task<DiscordBotJoinedGuildModel> GetOrAddAsync(Guid discordBotGuid, SocketGuild guild, CancellationToken cancellationToken = default);
}
