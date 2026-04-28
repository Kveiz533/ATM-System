namespace BankApi.Presentation.Http.Models.Requests;

public sealed class WithdrawRequest
{
    public required Guid SessionId { get; init; }

    public required decimal Money { get; init; }
}