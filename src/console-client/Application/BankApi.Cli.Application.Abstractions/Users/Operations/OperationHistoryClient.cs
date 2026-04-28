using BankApi.Cli.Application.Abstractions.Users.Models;

namespace BankApi.Cli.Application.Abstractions.Users.Operations;

public static class OperationHistoryClient
{
    public readonly record struct Request(
        Guid SessionId);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success(
            IReadOnlyCollection<OperationHistoryDto> History) : Result;

        public sealed record Failure(string Message) : Result;
    }
}