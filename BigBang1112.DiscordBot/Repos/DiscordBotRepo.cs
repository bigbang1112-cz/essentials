using BigBang1112.DiscordBot.Attributes;
using BigBang1112.DiscordBot.Data;
using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Extensions;
using BigBang1112.Repos;

namespace BigBang1112.DiscordBot.Repos;

public class DiscordBotRepo : Repo<DiscordBotModel>, IDiscordBotRepo
{
    private readonly DiscordBotContext _context;

    public DiscordBotRepo(DiscordBotContext context) : base(context)
    {
        _context = context;
    }

    public async Task<DiscordBotModel> GetOrAddAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        return await _context.DiscordBots.FirstOrAddAsync(x => x.Guid == guid,
            () => new DiscordBotModel { Guid = guid }, expiration: TimeSpan.FromHours(1), cancellationToken);
    }

    public async Task<DiscordBotModel> AddOrUpdateAsync(DiscordBotAttribute attribute, CancellationToken cancellationToken = default)
    {
        var discordBotModel = await GetOrAddAsync(attribute.Guid, cancellationToken);

        discordBotModel.Name = attribute.Name;

        return discordBotModel;
    }
}
