using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Repos;

public interface IDiscordUserRepo : IRepo<DiscordUserModel>
{
    Task<DiscordUserModel> AddOrUpdateAsync(DiscordBotModel bot, SocketUser user, CancellationToken cancellationToken = default);
    Task<DiscordUserModel> AddOrUpdateAsync(Guid botGuid, SocketUser user, CancellationToken cancellationToken = default);
}
