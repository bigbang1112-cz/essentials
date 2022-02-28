using System.Text.Json.Serialization;

namespace BigBang1112.Models;

public class ManiaPlanetPlayer
{
    public string Login { get; init; } = default!;
    public string Nickname { get; init; } = default!;

    [JsonPropertyName("path")]
    public string Zone { get; init; } = default!;
}
