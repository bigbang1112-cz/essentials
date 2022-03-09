using Discord;
using Discord.WebSocket;

namespace BigBang1112.Models;

public interface IDiscordBotCommand
{
    Task<DiscordBotMessage> ExecuteAsync(SocketSlashCommand slashCommand);
    Task AutocompleteAsync(SocketAutocompleteInteraction interaction, AutocompleteOption option);
    IEnumerable<SlashCommandOptionBuilder> YieldOptions();
    Task<DiscordBotMessage> SelectMenuAsync(SocketMessageComponent messageComponent, IReadOnlyCollection<string> values);
}
