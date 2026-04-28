using BankApi.Application.Contracts.Sessions.Models;

namespace BankApi.Application.Contracts.Sessions.Operations;

public static class LogInAdmin
{
    public readonly record struct Request(string SystemPassword);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(SessionDto Session) : Response;

        public sealed record Failure(string Message) : Response;
    }
}