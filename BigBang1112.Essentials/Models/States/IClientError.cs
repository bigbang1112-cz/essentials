namespace BigBang1112.Models.States;

public interface IClientError
{
    int StatusCode { get; }
    string Message { get; }
}
