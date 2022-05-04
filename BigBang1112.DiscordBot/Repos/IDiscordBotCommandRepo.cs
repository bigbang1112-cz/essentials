using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;

namespace BigBang1112.DiscordBot.Repos;

public interface IDiscordBotCommandRepo : IRepo<DiscordBotCommandModel>
{
    Task AddOrUpdateAsync(DiscordBotModel bot, string fullCommandName, CancellationToken cancellationToken = default);
    Task AddOrUpdateAsync(Guid botGuid, string fullCommandName, CancellationToken cancellationToken = default);
}
