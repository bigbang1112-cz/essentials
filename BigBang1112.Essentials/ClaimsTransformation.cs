using AspNet.Security.OAuth.Discord;
using AspNet.Security.OAuth.GitHub;
using BigBang1112.Data;
using BigBang1112.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using BigBang1112.Models.Db;
using BigBang1112.Models;
using AspNet.Security.OAuth.Twitter;

namespace BigBang1112;

public class ClaimsTransformation : IClaimsTransformation
{
    private readonly IAccountsUnitOfWork _accountsUnitOfWork;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _config;

    public ClaimsTransformation(IAccountsUnitOfWork accountsUnitOfWork, IMemoryCache cache, IConfiguration config)
    {
        _accountsUnitOfWork = accountsUnitOfWork;
        _cache = cache;
        _config = config;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity)
        {
            return principal;
        }

        var claims = identity.Claims.ToLookup(x => x.Type, x => x.Value);

        var nameIdentifier = claims[ClaimTypes.NameIdentifier].FirstOrDefault();

        if (nameIdentifier is null || identity.AuthenticationType is null)
        {
            return principal;
        }

        var cacheKey = $"Account_{identity.AuthenticationType}_{nameIdentifier}";

        var accountInfoOrNull = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

            var account = identity.AuthenticationType switch
            {
                ManiaPlanetAuthenticationDefaults.AuthenticationScheme => await SetupManiaPlanetAsync(claims),
                TrackmaniaAuthenticationDefaults.AuthenticationScheme => await SetupTrackmaniaAsync(claims),
                DiscordAuthenticationDefaults.AuthenticationScheme => await SetupDiscordAsync(claims),
                GitHubAuthenticationDefaults.AuthenticationScheme => await SetupGitHubAsync(claims),
                TwitterAuthenticationDefaults.AuthenticationScheme => await SetupTwitterAsync(claims),
                _ => null,
            };

            if (account is null)
            {
                return null;
            }

            account.LastSeenOn = DateTime.UtcNow;

            var isAdmin = await _accountsUnitOfWork.Admins.IsAccountAdminAsync(account);

            await _accountsUnitOfWork.SaveAsync();

            return new AccountInfo(account.Guid);
        });

        if (accountInfoOrNull is null)
        {
            return principal;
        }

        var accountInfo = accountInfoOrNull;

        var stringGuid = accountInfo.Guid.ToString();

        identity.AddClaim(new Claim(BigBang1112AuthenticationConstants.Claims.AccountUuid, stringGuid));

        if (_config["SuperAdmin"] == stringGuid)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, BigBang1112AuthenticationConstants.Claims.SuperAdminRole));
        }

        identity.AddClaim(new Claim(ClaimTypes.Role, accountInfo.Role.ToString()));

        return principal;
    }

    private async Task<AccountModel?> SetupTwitterAsync(ILookup<string, string> claims)
    {
        var nameIdentifier = claims[ClaimTypes.NameIdentifier].First();
        var userId = ulong.Parse(nameIdentifier);

        var auth = await _accountsUnitOfWork.TwitterAuth.GetOrAddAsync(userId);

        var name = claims[ClaimTypes.Name].First();
        auth.Name = name;

        return auth.Account;
    }

    private async Task<AccountModel?> SetupGitHubAsync(ILookup<string, string> claims)
    {
        var nameIdentifier = claims[ClaimTypes.NameIdentifier].First();
        var uid = uint.Parse(nameIdentifier);

        var auth = await _accountsUnitOfWork.GitHubAuth.GetOrAddAsync(uid);

        var name = claims[ClaimTypes.Name].First();
        auth.Name = name;

        var displayName = claims[GitHubAuthenticationConstants.Claims.Name].First();
        auth.DisplayName = displayName;

        var email = claims[ClaimTypes.Email].FirstOrDefault();
        auth.Email = email;

        return auth.Account;
    }

    private async Task<AccountModel?> SetupDiscordAsync(ILookup<string, string> claims)
    {
        var nameIdentifier = claims[ClaimTypes.NameIdentifier].First();
        var snowflake = ulong.Parse(nameIdentifier);

        var auth = await _accountsUnitOfWork.DiscordAuth.GetOrAddAsync(snowflake);

        var name = claims[ClaimTypes.Name].First();

        if (!string.IsNullOrWhiteSpace(name))
        {
            auth.Name = name;
        }

        var discriminatorStr = claims[DiscordAuthenticationConstants.Claims.Discriminator].First();
        var discriminator = short.Parse(discriminatorStr);
        auth.Discriminator = discriminator;

        var avatarHash = claims[DiscordAuthenticationConstants.Claims.AvatarHash].First();
        auth.AvatarHash = avatarHash;

        return auth.Account;
    }

    private async Task<AccountModel?> SetupManiaPlanetAsync(ILookup<string, string> claims)
    {
        var nameIdentifier = claims[ClaimTypes.NameIdentifier].First();

        var auth = await _accountsUnitOfWork.ManiaPlanetAuth.GetOrAddAsync(nameIdentifier);

        var nickname = claims[ClaimTypes.Name].First();

        if (!string.IsNullOrWhiteSpace(nickname))
        {
            auth.Nickname = nickname;
        }

        var zone = claims[ManiaPlanetAuthenticationConstants.Claims.Zone].First();

        if (!string.IsNullOrWhiteSpace(zone))
        {
            auth.Zone = await _accountsUnitOfWork.Zones.GetOrAddAsync(zone);
            auth.Zone.IsMP = true;
        }

        return auth.Account;
    }

    private async Task<AccountModel?> SetupTrackmaniaAsync(ILookup<string, string> claims)
    {
        var nameIdentifier = claims[ClaimTypes.NameIdentifier].First();
        var loginGuid = new Guid(nameIdentifier);

        var auth = await _accountsUnitOfWork.TrackmaniaAuth.GetOrAddAsync(loginGuid);

        var nickname = claims[ClaimTypes.Name].First();

        if (!string.IsNullOrWhiteSpace(nickname))
        {
            auth.Nickname = nickname;
        }

        return auth.Account;
    }
}
