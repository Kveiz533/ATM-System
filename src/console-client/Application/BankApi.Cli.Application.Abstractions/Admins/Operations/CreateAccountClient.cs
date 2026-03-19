namespace BankApi.Cli.Application.Abstractions.Admins.Operations;

public static class CreateAccountClient
{
    public readonly record struct Request(
        Guid SessionId,
        string AccountNumber,
        string PinCode);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success : Result;

        public sealed record Failure(string Message) : Result;
    }
}