using AspNet.Security.OAuth.Discord;
using AspNet.Security.OAuth.GitHub;
using BigBang1112.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BigBang1112;

public static class AuthExtensions
{
    public static AuthenticationBuilder AddMultiAuth(this AuthenticationBuilder builder, IConfiguration config, ILogger? logger = null)
    {
        var maniaPlanetClientId = ReportIfMissing("OAuth2:ManiaPlanet:Id", config, logger);
        var maniaPlanetSecret = ReportIfMissing("OAuth2:ManiaPlanet:Secret", config, logger);
        var trackmaniaClientId = ReportIfMissing("OAuth2:Trackmania:Id", config, logger);
        var trackmaniaSecret = ReportIfMissing("OAuth2:Trackmania:Secret", config, logger);
        var gitHubClientId = ReportIfMissing("OAuth2:GitHub:Id", config, logger);
        var gitHubSecret = ReportIfMissing("OAuth2:GitHub:Secret", config, logger);
        var discordClientId = ReportIfMissing("OAuth2:Discord:Id", config, logger);
        var discordSecret = ReportIfMissing("OAuth2:Discord:Secret", config, logger);
        var twitterKey = ReportIfMissing("OAuth2:Twitter:Key", config, logger);
        var twitterSecret = ReportIfMissing("OAuth2:Twitter:Secret", config, logger);

        builder.AddCookie(options =>
        {
            options.LoginPath = "/login";
            options.LogoutPath = "/logout";
        });

        AddManiaPlanet(builder, maniaPlanetClientId, maniaPlanetSecret);
        AddTrackmania(builder, trackmaniaClientId, trackmaniaSecret);
        AddGitHub(builder, gitHubClientId, gitHubSecret);
        AddDiscord(builder, discordClientId, discordSecret);
        AddTwitter(builder, twitterKey, twitterSecret);

        return builder;
    }

    private static void AddManiaPlanet(AuthenticationBuilder builder, string maniaPlanetClientId, string maniaPlanetSecret)
    {
        if (string.IsNullOrWhiteSpace(maniaPlanetClientId) || string.IsNullOrWhiteSpace(maniaPlanetSecret))
        {
            return;
        }

        builder.AddManiaPlanet(options =>
        {
            options.ClientId = maniaPlanetClientId;
            options.ClientSecret = maniaPlanetSecret;
            options.CallbackPath = "/login/maniaplanet/redirect";

            Array.ForEach(new[] { "basic", "dedicated", "titles", "maps" }, options.Scope.Add);
        });
    }

    private static void AddTrackmania(AuthenticationBuilder builder, string trackmaniaClientId, string trackmaniaSecret)
    {
        if (string.IsNullOrWhiteSpace(trackmaniaClientId) || string.IsNullOrWhiteSpace(trackmaniaSecret))
        {
            return;
        }

        builder.AddTrackmania(options =>
        {
            options.ClientId = trackmaniaClientId;
            options.ClientSecret = trackmaniaSecret;
            options.CallbackPath = "/login/trackmania/redirect";
        });
    }

    private static void AddGitHub(AuthenticationBuilder builder, string gitHubClientId, string gitHubSecret)
    {
        if (string.IsNullOrWhiteSpace(gitHubClientId) || string.IsNullOrWhiteSpace(gitHubSecret))
        {
            return;
        }

        builder.AddGitHub(GitHubAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.ClientId = gitHubClientId;
            options.ClientSecret = gitHubSecret;
            options.CallbackPath = "/login/github/redirect";
            options.SaveTokens = true;
        });
    }

    private static void AddDiscord(AuthenticationBuilder builder, string discordClientId, string discordSecret)
    {
        if (string.IsNullOrWhiteSpace(discordClientId) || string.IsNullOrWhiteSpace(discordSecret))
        {
            return;
        }

        builder.AddDiscord(DiscordAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.ClientId = discordClientId;
            options.ClientSecret = discordSecret;
            options.CallbackPath = "/login/discord/redirect";
            options.SaveTokens = true;

            options.Events.OnAccessDenied = context =>
            {
                context.Response.Redirect("/login");
                context.HandleResponse();

                return Task.CompletedTask;
            };
        });
    }

    private static void AddTwitter(AuthenticationBuilder builder, string twitterKey, string twitterSecret)
    {
        if (string.IsNullOrWhiteSpace(twitterKey) || string.IsNullOrWhiteSpace(twitterSecret))
        {
            return;
        }
        
        builder.AddTwitter(options =>
        {
            options.ClientId = twitterKey;
            options.ClientSecret = twitterSecret;
            options.CallbackPath = "/login/twitter/redirect";
        });
    }

    private static string ReportIfMissing(string key, IConfiguration config, ILogger? logger = null)
    {
        var value = config[key];

        if (string.IsNullOrWhiteSpace(value))
            logger?.LogWarning("{key} missing!", key);

        return value;
    }
}
