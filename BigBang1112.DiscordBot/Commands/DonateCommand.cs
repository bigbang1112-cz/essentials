using BigBang1112.DiscordBot.Attributes;
using BigBang1112.DiscordBot.Models;
using Discord;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Commands;

[DiscordBotCommand("donate", "Support the bot and other projects by BigBang1112.")]
public class DonateCommand : DiscordBotCommand
{
    public DonateCommand(DiscordBotService discordBotService) : base(discordBotService)
    {
        
    }

    public override Task<DiscordBotMessage> ExecuteAsync(SocketInteraction slashCommand)
    {
        var embed = new EmbedBuilder()
            .WithTitle($"Support the project via small donate")
            .WithDescription("By donating, you can be sure I'll actively invest in new Trackmania discoveries. No matter what though, I will still continue doing free/open-source content as much as I am able to. Thank you for your generous support. :blue_heart:")
            .WithUrl("https://www.paypal.com/paypalme/bigbang1112")
            .Build();

        return Task.FromResult(new DiscordBotMessage(embed));
    }
}