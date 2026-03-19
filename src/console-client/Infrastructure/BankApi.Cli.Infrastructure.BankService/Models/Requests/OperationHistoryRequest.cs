namespace BankApi.Cli.Infrastructure.BankService.Models.Requests;

public sealed record OperationHistoryRequest(
    Guid SessionId,
    string? PageToken,
    int PageSize);
