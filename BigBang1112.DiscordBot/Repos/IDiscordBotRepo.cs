using BigBang1112.DiscordBot.Attributes;
using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;

namespace BigBang1112.DiscordBot.Repos;

public interface IDiscordBotRepo : IRepo<DiscordBotModel>
{
    Task<DiscordBotModel> AddOrUpdateAsync(DiscordBotAttribute attribute, CancellationToken cancellationToken = default);
    Task<DiscordBotModel> GetOrAddAsync(Guid guid, CancellationToken cancellationToken = default);
}
