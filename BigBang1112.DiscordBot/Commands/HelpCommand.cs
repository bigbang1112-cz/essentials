using BigBang1112.DiscordBot.Attributes;
using BigBang1112.DiscordBot.Models;
using Discord;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Commands;

[DiscordBotCommand("help", "Sends some help.")]
public class HelpCommand : DiscordBotCommand
{
    public HelpCommand(DiscordBotService discordBotService) : base(discordBotService)
    {

    }

    public override async Task<DiscordBotMessage> ExecuteAsync(SocketInteraction slashCommand)
    {
        var embedBuilder = new EmbedBuilder
        {
            Title = "TMWR - The Ultimate Trackmania WR Discord Bot",
            Description = "With this bot, you can quickly check any world records, history of world records, graphs, or replay parameters in the future.\n\nRegarding visibility of the command executions"
        };

        return new DiscordBotMessage
        {
            Message = "No help yet.",
            Ephemeral = true
        };
    }
}
