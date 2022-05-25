using BigBang1112.DiscordBot.Data;
using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Extensions;
using BigBang1112.Repos;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.DiscordBot.Repos;

public class DiscordBotJoinedGuildRepo : Repo<DiscordBotJoinedGuildModel>, IDiscordBotJoinedGuildRepo
{
    private readonly DiscordBotContext _context;

    public DiscordBotJoinedGuildRepo(DiscordBotContext context) : base(context)
    {
        _context = context;
    }

    public async Task<DiscordBotJoinedGuildModel?> GetByBotAndTextChannelAsync(Guid discordBotGuid, SocketTextChannel textChannel, CancellationToken cancellationToken = default)
    {
        var discordBotModel = await new DiscordBotRepo(_context).GetOrAddAsync(discordBotGuid, cancellationToken);

        if (discordBotModel is null)
        {
            return null;
        }

        return await GetByBotAndGuildAsync(discordBotModel, textChannel.Guild, cancellationToken);
    }

    public async Task<DiscordBotJoinedGuildModel?> GetByBotAndGuildAsync(DiscordBotModel discordBotModel, SocketGuild guild, CancellationToken cancellationToken = default)
    {
        return await _context.DiscordBotJoinedGuilds.FirstOrDefaultAsync(x => x.Bot == discordBotModel && x.Guild.Snowflake == guild.Id, cancellationToken);
    }

    public async Task<DiscordBotJoinedGuildModel> GetOrAddAsync(DiscordBotModel discordBot, SocketGuild guild, CancellationToken cancellationToken = default)
    {
        var discordGuildModel = await new DiscordBotGuildRepo(_context).AddOrUpdateAsync(guild, cancellationToken);

        return await _context.DiscordBotJoinedGuilds.FirstOrAddAsync(x => x.Guild.Id == discordGuildModel.Id,
            () => new DiscordBotJoinedGuildModel { Bot = discordBot, Guild = discordGuildModel },
            cancellationToken: cancellationToken);
    }

    public async Task<DiscordBotJoinedGuildModel> GetOrAddAsync(Guid discordBotGuid, SocketGuild guild, CancellationToken cancellationToken = default)
    {
        var discordBot = await new DiscordBotRepo(_context).GetOrAddAsync(discordBotGuid, cancellationToken);

        return await GetOrAddAsync(discordBot, guild, cancellationToken);
    }
}
