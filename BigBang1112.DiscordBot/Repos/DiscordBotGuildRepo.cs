using BigBang1112.DiscordBot.Data;
using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Extensions;
using BigBang1112.Repos;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Repos;

public class DiscordBotGuildRepo : Repo<DiscordBotGuildModel>, IDiscordBotGuildRepo
{
    private readonly DiscordBotContext _context;

    public DiscordBotGuildRepo(DiscordBotContext context) : base(context)
    {
        _context = context;
    }

    public async Task<DiscordBotGuildModel> GetOrAddAsync(SocketGuild guild, CancellationToken cancellationToken = default)
    {
        return await _context.DiscordBotGuilds.FirstOrAddAsync(x => x.Snowflake == guild.Id,
            () => new DiscordBotGuildModel { Snowflake = guild.Id, Name = guild.Name },
            cancellationToken: cancellationToken);
    }

    public async Task<DiscordBotGuildModel> AddOrUpdateAsync(SocketGuild guild, CancellationToken cancellationToken = default)
    {
        var discordGuildModel = await GetOrAddAsync(guild, cancellationToken);

        discordGuildModel.Name = guild.Name;

        return discordGuildModel;
    }
}
