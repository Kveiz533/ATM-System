namespace BankApi.Cli.Infrastructure.BankService.Models.Responses;

public sealed record OperationHistoryResponse(
    IReadOnlyList<OperationHistoryResponse.OperationHistoryDto> History,
    string? PageToken)
{
    public sealed record OperationHistoryDto(
        long BankOperationId,
        long AccountId,
        DateTimeOffset Time,
        string BankOperationType,
        decimal Balance);
}