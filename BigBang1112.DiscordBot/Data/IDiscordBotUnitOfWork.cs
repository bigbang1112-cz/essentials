using BigBang1112.Data;
using BigBang1112.DiscordBot.Repos;

namespace BigBang1112.DiscordBot.Data;

public interface IDiscordBotUnitOfWork : IUnitOfWork
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
    public IReportChannelRepo ReportChannels { get; }
    public IReportChannelMessageRepo ReportChannelMessages { get; }
}
