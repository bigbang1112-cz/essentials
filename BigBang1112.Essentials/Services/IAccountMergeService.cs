using BigBang1112.Models.Db;
using System.Security.Claims;

namespace BigBang1112.Services;

public interface IAccountMergeService
{
    ValueTask<(AccountModel accountToMergeInto, AccountModel currentAccount)?> GetAccountsToMergeAsync(ClaimsPrincipal user);
    Task MergeAccountsAsync(ClaimsPrincipal user);
}
