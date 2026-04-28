using BankApi.Application.Contracts.Accounts.Models;

namespace BankApi.Application.Contracts.Accounts.Operations;

public static class OperationHistory
{
    public readonly record struct PageToken(long Key);

    public readonly record struct Request(
        Guid SessionId,
        int PageSize,
        PageToken? PageToken);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(
            IReadOnlyCollection<OperationHistoryDto> History,
            PageToken? PageToken) : Response;

        public sealed record Failure(string Message) : Response;
    }
}