using BankApi.Cli.Application.Abstractions.Users.Models;
using BankApi.Cli.Application.Contracts.Users.Models;

namespace BankApi.Cli.Application.Mapping;

public static class OperationHistoryMappingExtension
{
    public static IReadOnlyCollection<OperationHistoryEntity> MapToEntity(this IEnumerable<OperationHistoryDto> bankOperations)
    {
        return bankOperations.Select(dto =>
            new OperationHistoryEntity(
                BankOperationId: dto.BankOperationId,
                AccountId: dto.AccountId,
                Time: dto.Time,
                BankOperationType: dto.BankOperationType,
                Balance: dto.Balance))
            .ToList();
    }
}
