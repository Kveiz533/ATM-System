namespace BankApi.Cli.Application.Contracts.Users.Operations;

public static class Balance
{
    public abstract record Response
    {
        private Response() { }

        public sealed record Success(decimal Balance) : Response;

        public sealed record Failure(string Message) : Response;
    }
}