using BankApi.Cli.Application.Contracts.Users.Models;

namespace BankApi.Cli.Application.Contracts.Users.Operations;

public static class OperationHistory
{
    public readonly record struct Request();

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(
            IReadOnlyCollection<OperationHistoryEntity> History) : Response;

        public sealed record Failure(string Message) : Response;
    }
}