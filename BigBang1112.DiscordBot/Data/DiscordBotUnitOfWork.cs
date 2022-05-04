using BigBang1112.Data;
using BigBang1112.DiscordBot.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BigBang1112.DiscordBot.Data;

public class DiscordBotUnitOfWork : UnitOfWork, IDiscordBotUnitOfWork
{
    public IDiscordBotChannelRepo DiscordBotChannels { get; }
    public IDiscordBotCommandRepo DiscordBotCommands { get; }
    public IDiscordBotCommandVisibilityRepo DiscordBotCommandVisibilities { get; }
    public IDiscordBotGuildRepo DiscordBotGuilds { get; }
    public IDiscordBotJoinedGuildRepo DiscordBotJoinedGuilds { get; }
    public IDiscordBotRepo DiscordBots { get; }
    public IDiscordUserRepo DiscordUsers { get; }
    public IFeedbackRepo Feedbacks { get; }
    public IMemeRepo Memes { get; }
    public IPingMessageRepo PingMessages { get; }
    public IWorldRecordReportChannelRepo WorldRecordReportChannels { get; }

    public DiscordBotUnitOfWork(DiscordBotContext context, ILogger<UnitOfWork> logger) : base(context, logger)
    {
        DiscordBotChannels = new DiscordBotChannelRepo(context);
        DiscordBotCommands = new DiscordBotCommandRepo(context);
        DiscordBotCommandVisibilities = new DiscordBotCommandVisibilityRepo(context);
        DiscordBotGuilds = new DiscordBotGuildRepo(context);
        DiscordBotJoinedGuilds = new DiscordBotJoinedGuildRepo(context);
        DiscordBots = new DiscordBotRepo(context);
        DiscordUsers = new DiscordUserRepo(context);
        Feedbacks = new FeedbackRepo(context);
        Memes = new MemeRepo(context);
        PingMessages = new PingMessageRepo(context);
        WorldRecordReportChannels = new WorldRecordReportChannelRepo(context);
    }
}
