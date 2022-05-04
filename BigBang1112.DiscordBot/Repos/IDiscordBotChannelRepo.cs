using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Repos;

public interface IDiscordBotChannelRepo : IRepo<DiscordBotChannelModel>
{
    Task<DiscordBotChannelModel?> GetAsync(ulong snowflake, CancellationToken cancellationToken = default);
    Task<DiscordBotChannelModel> GetOrAddAsync(SocketTextChannel textChannel, CancellationToken cancellationToken = default);
}
