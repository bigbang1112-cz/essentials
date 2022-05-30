using System.Text.Json;
using System.Text.Json.Serialization;
using Discord;

namespace BigBang1112.DiscordBot.Models;

public record AutoThreadOptions(ThreadArchiveDuration ArchiveDuration = ThreadArchiveDuration.OneDay)
{
    internal static JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}
