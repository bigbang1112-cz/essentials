using BigBang1112.Attributes.DiscordBot;
using BigBang1112.Models.DiscordBot;
using BigBang1112.Services;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Commands;

[DiscordBotCommand("visibility")]
public partial class VisibilityCommand : DiscordBotCommand
{
    public VisibilityCommand(DiscordBotService discordBotService) : base(discordBotService)
    {

    }

    public override Task<DiscordBotMessage> ExecuteAsync(SocketSlashCommand slashCommand)
    {
        throw new NotImplementedException();
    }
}
