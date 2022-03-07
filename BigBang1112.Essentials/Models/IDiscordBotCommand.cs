using Discord;
using Discord.WebSocket;

namespace BigBang1112.Models;

public interface IDiscordBotCommand
{
    Task ExecuteAsync(SocketSlashCommand slashCommand);
    Task AutocompleteAsync(SocketAutocompleteInteraction interaction, AutocompleteOption option);
    IEnumerable<SlashCommandOptionBuilder> YieldOptions();
    Task SelectMenuAsync(SocketMessageComponent messageComponent, IReadOnlyCollection<string> values);
}
