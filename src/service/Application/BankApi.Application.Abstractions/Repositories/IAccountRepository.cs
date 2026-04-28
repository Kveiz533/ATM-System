using BankApi.Application.Abstractions.Queries;
using BankApi.Domain.Accounts;

namespace BankApi.Application.Abstractions.Repositories;

public interface IAccountRepository
{
    IAsyncEnumerable<Account> AddAsync(IReadOnlyCollection<Account> accounts, CancellationToken cancellationToken);

    IAsyncEnumerable<Account> UpdateAsync(IReadOnlyCollection<Account> accounts, CancellationToken cancellationToken);

    IAsyncEnumerable<Account> QueryAsync(AccountQuery accountQuery, CancellationToken cancellationToken);
}