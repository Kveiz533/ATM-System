using BankApi.Application.Contracts.Admins.Models;

namespace BankApi.Application.Contracts.Admins.Operations;

public static class CreateAccount
{
    public readonly record struct Request(
        Guid SessionId,
        string AccountNumber,
        string PinCode);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(AccountDto AccountNumber) : Response;

        public sealed record Failure(string Message) : Response;
    }
}