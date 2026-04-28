namespace BankApi.Cli.Application.Abstractions.Admins.Operations;

public static class LogInAdminClient
{
    public readonly record struct Request(string SystemPassword);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success(Guid SessionId) : Result;

        public sealed record Failure(string Message) : Result;
    }
}