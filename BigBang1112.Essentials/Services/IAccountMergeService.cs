using BigBang1112.Models.Db;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BigBang1112.Services;

public interface IAccountMergeService
{
    ValueTask<(AccountModel accountToMergeInto, AccountModel currentAccount)?> GetAccountsToMergeAsync(ClaimsPrincipal user, ISession session);
    Task MergeAccountsAsync(ClaimsPrincipal user, ISession session);
}
