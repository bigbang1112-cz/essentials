using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Repos;

public interface IDiscordBotGuildRepo : IRepo<DiscordBotGuildModel>
{
    Task<DiscordBotGuildModel> AddOrUpdateAsync(SocketGuild guild, CancellationToken cancellationToken = default);
    Task<DiscordBotGuildModel> GetOrAddAsync(SocketGuild guild, CancellationToken cancellationToken = default);
}
