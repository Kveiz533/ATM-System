namespace BankApi.Cli.Application.Contracts.Users.Operations;

public static class Deposit
{
    public readonly record struct Request(decimal Amount);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(decimal Balance) : Response;

        public sealed record Failure(string Message) : Response;
    }
}