using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace BigBang1112.Controllers;

[Route("logout")]
[ApiExplorerSettings(IgnoreApi = true)]
public class LogoutController : Controller
{
    public IActionResult Logout() => SignOut(new AuthenticationProperties { RedirectUri = "/" });
}
