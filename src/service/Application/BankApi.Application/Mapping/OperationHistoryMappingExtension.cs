using BankApi.Application.Contracts.Accounts.Models;
using BankApi.Domain.BankOperations;

namespace BankApi.Application.Mapping;

public static class OperationHistoryMappingExtension
{
    public static IReadOnlyCollection<OperationHistoryDto> MapToDto(this IEnumerable<BankOperation> bankOperations)
    {
        return bankOperations.Select(operation =>
            new OperationHistoryDto(
                BankOperationId: operation.Id.Value,
                AccountId: operation.AccountId.Value,
                Time: operation.Time,
                BankOperationType: operation.BankOperationType.ToString(),
                Balance: operation.Balance.Value)).
            ToList();
    }
}
