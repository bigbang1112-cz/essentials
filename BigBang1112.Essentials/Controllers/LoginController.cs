using AspNet.Security.OAuth.Discord;
using AspNet.Security.OAuth.GitHub;
using BigBang1112.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BigBang1112.Controllers;

[Route("login")]
[ApiExplorerSettings(IgnoreApi = true)]
public class LoginController : ControllerBase
{
    [HttpGet("discord")]
    public IActionResult Discord() => Challenge(DiscordAuthenticationDefaults.AuthenticationScheme);

    [HttpGet("github")]
    public IActionResult GitHub() => Challenge(GitHubAuthenticationDefaults.AuthenticationScheme);

    [HttpGet("twitter")]
    public IActionResult Twitter() => Challenge(TwitterDefaults.AuthenticationScheme);

    [HttpGet("maniaplanet")]
    public IActionResult ManiaPlanet() => Challenge(ManiaPlanetAuthenticationDefaults.AuthenticationScheme);

    [HttpGet("trackmania")]
    public IActionResult Trackmania() => Challenge(TrackmaniaAuthenticationDefaults.AuthenticationScheme);

    private ChallengeResult Challenge(string scheme)
    {
        var currentAccountUuid = User.Claims
            .FirstOrDefault(x => x.Type == BigBang1112AuthenticationConstants.Claims.AccountUuid)?.Value;

        var redirectUri = currentAccountUuid is null ? "/" : "/account/merge";

        return Challenge(new AuthenticationProperties { RedirectUri = redirectUri }, scheme);
    }
}
