using BigBang1112.DiscordBot.Attributes;
using BigBang1112.DiscordBot.Models;
using Discord;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Commands
{
    [DiscordBotCommand("github", "Shows GitHub information about the bot.")]
    public class GithubCommand : DiscordBotCommand
    {
        private readonly DiscordBotService _discordBotService;

        public GithubCommand(DiscordBotService discordBotService) : base(discordBotService)
        {
            _discordBotService = discordBotService;
        }

        public override Task<DiscordBotMessage> ExecuteAsync(SocketInteraction slashCommand)
        {
            var embed = new EmbedBuilder()
                .WithTitle($"{_discordBotService.GetName()} is on GitHub")
                .WithDescription("Don't hesitate to give it a star if you like the bot!")
                .WithUrl(_discordBotService.GetGitRepoUrl())
                .Build();

            return Task.FromResult(new DiscordBotMessage(embed));
        }
    }
}
