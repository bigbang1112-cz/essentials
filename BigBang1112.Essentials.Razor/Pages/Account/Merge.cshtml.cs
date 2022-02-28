using BigBang1112.Models.Db;
using BigBang1112.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BigBang1112.Razor.Pages.Account;

[Authorize]
public class MergeModel : PageModel
{
    private readonly IAccountMergeService _accountMergeService;

    public AccountModel AccountToMergeInto { get; set; } = default!;
    public AccountModel CurrentAccount { get; set; } = default!;

    public MergeModel(IAccountMergeService accountMergeService)
    {
        _accountMergeService = accountMergeService;
    }

    [HttpGet]
    public async Task<IActionResult> OnGet()
    {
        var accounts = await _accountMergeService.GetAccountsToMergeAsync(User, HttpContext.Session);

        if (!accounts.HasValue)
        {
            return Redirect();
        }

        AccountToMergeInto = accounts.Value.accountToMergeInto;
        CurrentAccount = accounts.Value.currentAccount;

        if (AccountToMergeInto == CurrentAccount) // Nothing to merge
        {
            return Redirect();
        }

        return Page();
    }

    private RedirectResult Redirect()
    {
        return Redirect("/");
    }

    [HttpPost]
    public async Task<IActionResult> OnPost()
    {
        await _accountMergeService.MergeAccountsAsync(User, HttpContext.Session);
        return Redirect("/");
    }
}
