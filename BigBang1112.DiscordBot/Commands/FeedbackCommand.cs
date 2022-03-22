using BigBang1112.DiscordBot.Attributes;
using BigBang1112.DiscordBot.Models;
using Discord;
using Discord.WebSocket;

namespace BigBang1112.DiscordBot.Commands;

[DiscordBotCommand("feedback", "Share some feedback!")]
public class FeedbackCommand : DiscordBotCommand
{
    public FeedbackCommand(DiscordBotService discordBotService) : base(discordBotService)
    {

    }

    public override Modal? ExecuteModal(SocketSlashCommand slashCommand)
    {
        return new ModalBuilder()
            .WithCustomId("feedback-modal")
            .WithTitle("Share feedback")
            .AddTextInput("Tell me something...", "feedback-text", TextInputStyle.Paragraph, " ...bugs, feature suggestions, review...", required: true)
            .Build();
    }
}
