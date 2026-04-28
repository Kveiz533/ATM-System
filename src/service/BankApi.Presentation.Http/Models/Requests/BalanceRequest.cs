namespace BankApi.Presentation.Http.Models.Requests;

public sealed class BalanceRequest
{
    public required Guid SessionId { get; init; }
}