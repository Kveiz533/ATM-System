namespace BankApi.Cli.Application.Contracts.Users.Operations;

public static class LogInUser
{
    public readonly record struct Request(
        string AccountNumber,
        string PinCode);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success : Response;

        public sealed record Failure(string Message) : Response;
    }
}