namespace BankApi.Cli.Application.Abstractions.Users.Operations;

public static class LogInUserClient
{
    public readonly record struct Request(
        string AccountNumber,
        string PinCode);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success(Guid SessionId) : Result;

        public sealed record Failure(string Message) : Result;
    }
}