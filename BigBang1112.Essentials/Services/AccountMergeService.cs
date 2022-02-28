using AspNet.Security.OAuth.Discord;
using AspNet.Security.OAuth.GitHub;
using BigBang1112.Auth;
using BigBang1112.Data;
using BigBang1112.Models.Db;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace BigBang1112.Services;

public class AccountMergeService : IAccountMergeService
{
    private readonly IAccountsRepo _repo;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _config;

    public AccountMergeService(IAccountsRepo repo, IMemoryCache cache, IConfiguration config)
    {
        _repo = repo;
        _cache = cache;
        _config = config;
    }

    public async Task MergeAccountsAsync(ClaimsPrincipal user, ISession session)
    {
        var accounts = await GetAccountsToMergeAsync(user, session);

        if (!accounts.HasValue)
        {
            return;
        }

        var accountToMergeInto = accounts.Value.accountToMergeInto;
        var currentAccount = accounts.Value.currentAccount;

        if (accountToMergeInto == currentAccount) // Nothing to merge
        {
            return;
        }

        accountToMergeInto.Trackmania ??= currentAccount.Trackmania;
        accountToMergeInto.ManiaPlanet ??= currentAccount.ManiaPlanet;
        accountToMergeInto.Discord ??= currentAccount.Discord;
        accountToMergeInto.GitHub ??= currentAccount.GitHub;
        accountToMergeInto.Twitter ??= currentAccount.Twitter;

        accountToMergeInto.LastSeenOn = currentAccount.LastSeenOn;

        currentAccount.MergedInto = accountToMergeInto;

        await _repo.SaveAsync();

        if (accountToMergeInto.ManiaPlanet is not null)
        {
            _cache.Remove($"Account_{ManiaPlanetAuthenticationDefaults.AuthenticationScheme}_{accountToMergeInto.ManiaPlanet.Login}");
        }

        if (accountToMergeInto.Trackmania is not null)
        {
            _cache.Remove($"Account_{TrackmaniaAuthenticationDefaults.AuthenticationScheme}_{accountToMergeInto.Trackmania.Login}");
        }

        if (accountToMergeInto.Discord is not null)
        {
            _cache.Remove($"Account_{DiscordAuthenticationDefaults.AuthenticationScheme}_{accountToMergeInto.Discord.Snowflake}");
        }

        if (accountToMergeInto.GitHub is not null)
        {
            _cache.Remove($"Account_{GitHubAuthenticationDefaults.AuthenticationScheme}_{accountToMergeInto.GitHub.Uid}");
        }

        if (accountToMergeInto.Twitter is not null)
        {
            _cache.Remove($"Account_{TwitterDefaults.AuthenticationScheme}_{accountToMergeInto.Twitter.UserId}");
        }

        session.Remove(SessionConstants.AccountUuidToMergeInto);
    }

    public async ValueTask<(AccountModel accountToMergeInto, AccountModel currentAccount)?> GetAccountsToMergeAsync(ClaimsPrincipal user, ISession session)
    {
        if (user.Identity is null)
        {
            return null;
        }

        var identity = user.Identity;

        var accountUuidToMergeInto = session.GetString(SessionConstants.AccountUuidToMergeInto);

        if (accountUuidToMergeInto is null)
        {
            return null;
        }

        var accountGuidToMergeInto = new Guid(accountUuidToMergeInto);

        var currentAccountUuid = user.Claims
            .FirstOrDefault(x => x.Type == BigBang1112AuthenticationConstants.Claims.AccountUuid)?.Value;

        if (currentAccountUuid is null)
        {
            return null;
        }

        var currentAccountGuid = new Guid(currentAccountUuid);

        var accountToMergeInto = await _repo.GetAccountByGuid(accountGuidToMergeInto);
        var currentAccount = await _repo.GetAccountByGuid(currentAccountGuid);

        if (accountToMergeInto is null || currentAccount is null)
        {
            return null;
        }

        // Higher tier role accounts should be the ones merged into
        if (currentAccount.Guid.ToString() == _config["SuperAdmin"]
        || (currentAccount.IsAdmin && !accountToMergeInto.IsAdmin))
        {
            (accountToMergeInto, currentAccount) = (currentAccount, accountToMergeInto);
        }

        return (accountToMergeInto, currentAccount);
    }
}
