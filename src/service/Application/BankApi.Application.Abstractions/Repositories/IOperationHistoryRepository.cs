using BankApi.Application.Abstractions.Queries;
using BankApi.Domain.BankOperations;

namespace BankApi.Application.Abstractions.Repositories;

public interface IOperationHistoryRepository
{
    IAsyncEnumerable<BankOperation> AddAsync(IReadOnlyCollection<BankOperation> bankOperations, CancellationToken cancellationToken);

    IAsyncEnumerable<BankOperation> QueryAsync(OperationHistoryQuery operationHistoryQuery, CancellationToken cancellationToken);
}