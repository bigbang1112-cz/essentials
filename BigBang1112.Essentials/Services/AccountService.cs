using BigBang1112.Auth;
using BigBang1112.Data;
using BigBang1112.Models.Db;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BigBang1112.Services;

public class AccountService
{
	private readonly AuthenticationStateProvider _auth;
    private readonly IAccountsUnitOfWork _accountsUnitOfWork;

    public AccountService(AuthenticationStateProvider auth, IAccountsUnitOfWork accountsUnitOfWork)
	{
		_auth = auth;
        _accountsUnitOfWork = accountsUnitOfWork;
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

		var fetchedAccount = await _accountsUnitOfWork.Accounts.GetByGuidAsync(guid.Value, cancellationToken);

		if (fetchedAccount is null)
		{
			return null;
		}

		var account = fetchedAccount.MergedInto ?? fetchedAccount;

		if (account.MergedInto is not null)
		{
			// This should not be called just yet when Gbx web gonna exist
			_accountsUnitOfWork.Accounts.Delete(fetchedAccount);
		}

		await SaveAsync(cancellationToken);

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

	public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
		await _accountsUnitOfWork.SaveAsync(cancellationToken);
    }
}
