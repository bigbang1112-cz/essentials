using BigBang1112.Auth;
using BigBang1112.Data;
using BigBang1112.Models.Db;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BigBang1112.Services;

public class AccountService
{
	private readonly AuthenticationStateProvider _auth;
    private readonly IAccountsRepo _repo;

    public AccountService(AuthenticationStateProvider auth, IAccountsRepo repo)
	{
		_auth = auth;
        _repo = repo;
    }

	public async Task<Guid?> GetUntrackedAccountGuidAsync()
	{
		var state = await _auth.GetAuthenticationStateAsync();

		return GetUntrackedAccountGuid(state.User);
	}

	public async Task<AccountModel?> GetAccountAsync(CancellationToken cancellationToken = default)
	{
		var guid = await GetUntrackedAccountGuidAsync();

		if (guid is null)
        {
			return null;
        }

		var fetchedAccount = await _repo.GetAccountByGuidAsync(guid.Value, cancellationToken);

		if (fetchedAccount is null)
		{
			return null;
		}

		var account = fetchedAccount.MergedInto ?? fetchedAccount;

		if (account.MergedInto is not null)
		{
			// This should not be called just yet when Gbx web gonna exist
			_repo.RemoveAccount(fetchedAccount);
		}

		return account;
	}

	public static Guid? GetUntrackedAccountGuid(ClaimsPrincipal user)
	{
		var accountUuid = user.Claims
			.FirstOrDefault(x => x.Type == BigBang1112AuthenticationConstants.Claims.AccountUuid)?.Value;

		if (accountUuid is null)
		{
			return null;
		}

		return new Guid(accountUuid);
	}
}
