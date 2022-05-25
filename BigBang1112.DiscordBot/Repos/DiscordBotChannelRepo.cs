using BigBang1112.DiscordBot.Data;
using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Extensions;
using BigBang1112.Repos;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.DiscordBot.Repos;

public class DiscordBotChannelRepo : Repo<DiscordBotChannelModel>, IDiscordBotChannelRepo
{
    private readonly DiscordBotContext _context;

    public DiscordBotChannelRepo(DiscordBotContext context) : base(context)
    {
        _context = context;
    }

    public async Task<DiscordBotChannelModel?> GetAsync(ulong snowflake, CancellationToken cancellationToken = default)
    {
        return await _context.DiscordBotChannels.FirstOrDefaultAsync(x => x.Snowflake == snowflake, cancellationToken);
    }

    public async Task<DiscordBotChannelModel> GetOrAddAsync(SocketTextChannel textChannel, CancellationToken cancellationToken = default)
    {
        var discordGuildModel = await new DiscordBotGuildRepo(_context)
            .GetOrAddAsync(textChannel.Guild, cancellationToken);

        return await _context.DiscordBotChannels.FirstOrAddAsync(x => x.Snowflake == textChannel.Id,
            () => new DiscordBotChannelModel { Snowflake = textChannel.Id, Name = textChannel.Name, Guild = discordGuildModel },
            cancellationToken: cancellationToken);
    }
}
