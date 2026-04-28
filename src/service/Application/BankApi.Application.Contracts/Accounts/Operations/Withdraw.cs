using BankApi.Application.Contracts.Accounts.Models;

namespace BankApi.Application.Contracts.Accounts.Operations;

public static class Withdraw
{
    public readonly record struct Request(Guid SessionId, decimal Amount);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(BalanceDto Balance) : Response;

        public sealed record Failure(string Message) : Response;
    }
}