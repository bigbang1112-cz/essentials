﻿using System.Text.Json.Serialization;

namespace BigBang1112.Models;

public class ManiaPlanetAuthResponse
{
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = default!;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = "";

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = "";
}
