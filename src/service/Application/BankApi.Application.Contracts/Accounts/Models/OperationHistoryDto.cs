namespace BankApi.Application.Contracts.Accounts.Models;

public sealed record OperationHistoryDto(
    long BankOperationId,
    long AccountId,
    DateTimeOffset Time,
    string BankOperationType,
    decimal Balance);