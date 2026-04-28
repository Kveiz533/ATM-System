using BankApi.Application.Contracts.Sessions.Models;

namespace BankApi.Application.Contracts.Sessions.Operations;

public static class LogInUser
{
    public readonly record struct Request(
        string AccountNumber,
        string PinCode);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(SessionDto Session) : Response;

        public sealed record Failure(string Message) : Response;
    }
}