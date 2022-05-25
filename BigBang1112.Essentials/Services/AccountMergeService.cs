using AspNet.Security.OAuth.Discord;
using AspNet.Security.OAuth.GitHub;
using AspNet.Security.OAuth.Twitter;
using BigBang1112.Auth;
using BigBang1112.Data;
using BigBang1112.Models.Db;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace BigBang1112.Services;

public class AccountMergeService : IAccountMergeService
{
    private readonly IAccountsUnitOfWork _accountsUnitOfWork;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _config;
    private readonly ProtectedSessionStorage _storage;

    public AccountMergeService(IAccountsUnitOfWork accountsUnitOfWork, IMemoryCache cache, IConfiguration config, ProtectedSessionStorage storage)
    {
        _accountsUnitOfWork = accountsUnitOfWork;
        _cache = cache;
        _config = config;
        _storage = storage;
    }

    public async Task MergeAccountsAsync(ClaimsPrincipal user)
    {
        var accounts = await GetAccountsToMergeAsync(user);

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

        await _accountsUnitOfWork.SaveAsync();

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
            _cache.Remove($"Account_{TwitterAuthenticationDefaults.AuthenticationScheme}_{accountToMergeInto.Twitter.UserId}");
        }

        await _storage.DeleteAsync(StorageConstants.AccountUuidToMergeInto);
    }

    public async ValueTask<(AccountModel accountToMergeInto, AccountModel currentAccount)?> GetAccountsToMergeAsync(ClaimsPrincipal user)
    {
        if (user.Identity is null)
        {
            return null;
        }

        var identity = user.Identity;

        var accountUuidToMergeInto = await _storage.GetAsync<Guid>(StorageConstants.AccountUuidToMergeInto);
        
        if (!accountUuidToMergeInto.Success)
        {
            return null;
        }

        var accountGuidToMergeInto = accountUuidToMergeInto.Value;

        var currentAccountUuid = user.Claims
            .FirstOrDefault(x => x.Type == BigBang1112AuthenticationConstants.Claims.AccountUuid)?.Value;

        if (currentAccountUuid is null)
        {
            return null;
        }

        var currentAccountGuid = new Guid(currentAccountUuid);

        var accountToMergeInto = await _accountsUnitOfWork.Accounts.GetByGuidAsync(accountGuidToMergeInto);
        var currentAccount = await _accountsUnitOfWork.Accounts.GetByGuidAsync(currentAccountGuid);

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
