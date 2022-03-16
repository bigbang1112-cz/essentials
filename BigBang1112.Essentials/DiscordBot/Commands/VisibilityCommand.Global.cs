using BigBang1112.Attributes.DiscordBot;
using BigBang1112.Models.DiscordBot;
using BigBang1112.Services;
using Discord;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Commands;

public partial class VisibilityCommand
{
    [DiscordBotSubCommand("global", "Gets or sets the global visibility of command executions for this bot.")]
    public class Global : DiscordBotCommand
    {
        [DiscordBotCommandOption("set", ApplicationCommandOptionType.Boolean, "Set to True or False.")]
        public bool? Set { get; set; }

        public Global(DiscordBotService discordBotService) : base(discordBotService)
        {

        }

        public override Task<DiscordBotMessage> ExecuteAsync(SocketSlashCommand slashCommand)
        {
            throw new NotImplementedException();
        }
    }
}
