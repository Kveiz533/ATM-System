namespace BankApi.Cli.Application.Contracts.Users.Models;

public sealed record OperationHistoryEntity(
    long BankOperationId,
    long AccountId,
    DateTimeOffset Time,
    string BankOperationType,
    decimal Balance);