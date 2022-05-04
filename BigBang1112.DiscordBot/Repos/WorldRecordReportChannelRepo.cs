using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigBang1112.DiscordBot.Data;
using BigBang1112.DiscordBot.Models.Db;
using BigBang1112.Repos;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.DiscordBot.Repos;

public class WorldRecordReportChannelRepo : Repo<WorldRecordReportChannelModel>, IWorldRecordReportChannelRepo
{
    private readonly DiscordBotContext _context;

    public WorldRecordReportChannelRepo(DiscordBotContext context) : base(context)
    {
        _context = context;
    }

    public async Task<WorldRecordReportChannelModel?> GetByJoinedGuildOrChannelAsync(DiscordBotJoinedGuildModel joinedGuild, ulong textChannelSnowflake, CancellationToken cancellationToken = default)
    {
        return await _context.WorldRecordReportChannels
            .Include(x => x.Channel)
            .Include(x => x.JoinedGuild)
            .FirstOrDefaultAsync(x => x.Channel.Snowflake == textChannelSnowflake && x.JoinedGuild == joinedGuild,
                cancellationToken);
    }

    public async Task<WorldRecordReportChannelModel?> GetByBotAndTextChannelAsync(Guid discordBotGuid, SocketTextChannel textChannel, CancellationToken cancellationToken = default)
    {
        var joinedGuild = await new DiscordBotJoinedGuildRepo(_context).GetByBotAndTextChannelAsync(discordBotGuid, textChannel, cancellationToken);

        if (joinedGuild is null)
        {
            return null;
        }

        return await GetByJoinedGuildOrChannelAsync(joinedGuild, textChannel.Id, cancellationToken);
    }

    public async Task AddOrUpdateAsync(Guid discordBotGuid, SocketTextChannel textChannel, bool set, CancellationToken cancellationToken = default)
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

            wrrChannelModel = new WorldRecordReportChannelModel
            {
                JoinedGuild = joinedGuild,
                Channel = channel
            };

            await _context.WorldRecordReportChannels.AddAsync(wrrChannelModel, cancellationToken);
        }

        wrrChannelModel.Enabled = set;
    }
}
