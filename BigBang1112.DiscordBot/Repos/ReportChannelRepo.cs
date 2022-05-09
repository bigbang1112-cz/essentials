using BigBang1112.DiscordBot.Data;
using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.DiscordBot.Repos;

public class ReportChannelRepo : Repo<ReportChannelModel>, IReportChannelRepo
{
    private readonly DiscordBotContext _context;

    public ReportChannelRepo(DiscordBotContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ReportChannelModel?> GetByJoinedGuildOrChannelAsync(DiscordBotJoinedGuildModel joinedGuild, ulong textChannelSnowflake, CancellationToken cancellationToken = default)
    {
        return await _context.ReportChannels
            .Include(x => x.Channel)
            .Include(x => x.JoinedGuild)
            .FirstOrDefaultAsync(x => x.Channel.Snowflake == textChannelSnowflake && x.JoinedGuild == joinedGuild,
                cancellationToken);
    }

    public async Task<ReportChannelModel?> GetByBotAndTextChannelAsync(Guid discordBotGuid, SocketTextChannel textChannel, CancellationToken cancellationToken = default)
    {
        var joinedGuild = await new DiscordBotJoinedGuildRepo(_context).GetByBotAndTextChannelAsync(discordBotGuid, textChannel, cancellationToken);

        if (joinedGuild is null)
        {
            return null;
        }

        return await GetByJoinedGuildOrChannelAsync(joinedGuild, textChannel.Id, cancellationToken);
    }

    public async Task AddOrUpdateAsync(Guid discordBotGuid, SocketTextChannel textChannel, string scopeSet, CancellationToken cancellationToken = default)
    {
        var joinedGuild = await new DiscordBotJoinedGuildRepo(_context).GetByBotAndTextChannelAsync(discordBotGuid, textChannel, cancellationToken);

        if (joinedGuild is null)
        {
            return;
        }

        var wrrChannelModel = await GetByJoinedGuildOrChannelAsync(joinedGuild, textChannel.Id, cancellationToken);

        if (wrrChannelModel is null)
        {
            var channel = await new DiscordBotChannelRepo(_context).GetOrAddAsync(textChannel, cancellationToken);

            wrrChannelModel = new ReportChannelModel
            {
                JoinedGuild = joinedGuild,
                Channel = channel
            };

            await _context.ReportChannels.AddAsync(wrrChannelModel, cancellationToken);
        }

        wrrChannelModel.Scope = scopeSet;
    }
}
