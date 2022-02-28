namespace BigBang1112.Models.States;

public class AccountForbidden : IClientError
{
    public int StatusCode => 403;
    public string Message => "This account is not allowed to get this information.";
}
