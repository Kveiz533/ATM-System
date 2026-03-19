using BankApi.Application.Contracts.Accounts.Models;

namespace BankApi.Presentation.Http.Models.Responses;

public sealed class OperationHistoryResponse
{
    public required IEnumerable<OperationHistoryDto> History { get; init; }

    public required string? PageToken { get; init; }
}