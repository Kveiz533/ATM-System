namespace BankApi.Cli.Application.Abstractions.Users.Operations;

public static class BalanceClient
{
    public readonly record struct Request(Guid SessionId);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success(decimal Balance) : Result;

        public sealed record Failure(string Message) : Result;
    }
}