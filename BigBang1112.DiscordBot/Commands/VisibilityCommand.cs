using BigBang1112.DiscordBot.Models;
using BigBang1112.DiscordBot.Attributes;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Commands;

[DiscordBotCommand("visibility")]
public partial class VisibilityCommand : DiscordBotCommand
{
    public VisibilityCommand(DiscordBotService discordBotService) : base(discordBotService)
    {

    }

    public override Task<DiscordBotMessage> ExecuteAsync(SocketInteraction slashCommand)
    {
        throw new NotImplementedException();
    }
}
