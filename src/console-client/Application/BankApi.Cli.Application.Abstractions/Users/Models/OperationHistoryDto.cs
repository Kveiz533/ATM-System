namespace BankApi.Cli.Application.Abstractions.Users.Models;

public sealed record OperationHistoryDto(
    long BankOperationId,
    long AccountId,
    DateTimeOffset Time,
    string BankOperationType,
    decimal Balance);