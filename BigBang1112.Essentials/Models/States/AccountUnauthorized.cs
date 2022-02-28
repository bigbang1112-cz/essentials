namespace BigBang1112.Models.States;

public class AccountUnauthorized : IClientError
{
    public int StatusCode => 401;
    public string Message => "The client cannot be identified. Please authenticate.";
}
