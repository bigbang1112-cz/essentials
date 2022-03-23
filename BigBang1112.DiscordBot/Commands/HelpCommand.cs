using BigBang1112.DiscordBot.Attributes;
using BigBang1112.DiscordBot.Models;
using Discord;
using Discord.WebSocket;
using System.Text;

namespace BigBang1112.DiscordBot.Commands;

[DiscordBotCommand("help", "Sends some help (or give help).")]
public class HelpCommand : DiscordBotCommand
{
    private readonly DiscordBotService _discordBotService;

    public HelpCommand(DiscordBotService discordBotService) : base(discordBotService)
    {
        _discordBotService = discordBotService;
    }

    public override Task<DiscordBotMessage> ExecuteAsync(SocketInteraction slashCommand)
    {
        var name = _discordBotService.GetName();
        var punchline = _discordBotService.GetPunchline();
        var description = _discordBotService.GetDescription();
        var ownerSnowflake = _discordBotService.GetOwnerDiscordSnowflake();

        var title = name is not null && punchline is not null
            ? $"{name} - {punchline}"
            : name ?? punchline ?? "Bot";

        var descBuilder = new StringBuilder();

        if (description is not null)
        {
            descBuilder.AppendLine(description);
            descBuilder.AppendLine();
        }

        descBuilder.AppendLine("By default, none of the command executions are visible. You can change this behaviour with two commands:");
        descBuilder.AppendLine();
        descBuilder.AppendLine("`/visibility global` - Get/set the global visibility for most of the command executions in every channel **(`set` requires Manage Guild permission)**.");
        descBuilder.AppendLine("`/visibility channel` - Get/set the visibility for most of the command executions for a specific channel **(`set` requires Manage Channels permission)**.");
        descBuilder.AppendLine();
        descBuilder.AppendLine($"If you have any issues with the bot, use the `/feedback` command, ping the bot on any Discord server, [create an issue on GitHub](https://github.com/bigbang1112-cz/world-record-report/issues), or DM me (<@{ownerSnowflake}>).\n**Bot cannot see regular DM messages.**");

        var embed = new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(descBuilder.ToString())
            .WithUrl(_discordBotService.GetGitRepoUrl())
            .AddField($"As already mentioned, {name ?? "this bot"} is on GitHub",
                "Don't hesitate to [give it a star](https://github.com/bigbang1112-cz/world-record-report) if you like using it!")
            .AddField("Support the project via small donate",
                "By [donating](https://www.paypal.com/paypalme/bigbang1112), you can be sure I'll actively invest in new Trackmania discoveries. No matter what though, I will still continue doing free/open-source content as much as I am able to. Thank you for your generous support. :blue_heart:")
            .Build();

        return Task.FromResult(new DiscordBotMessage(embed, ephemeral: true));
    }
}
