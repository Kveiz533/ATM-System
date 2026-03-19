using BankApi.Application.Abstractions.Queries;
using BankApi.Application.Abstractions.Repositories;
using BankApi.Domain.Accounts;

namespace BankApi.Application.Extensions;

public static class AccountRepositoryQueryExtensions
{
    public static async Task<Account?> FindAccountByIdAsync(
        this IAccountRepository accountRepository,
        AccountId accountId,
        CancellationToken cancellationToken)
    {
        var accountQuery = AccountQuery.Build(builder => builder
            .WithAccountId(accountId)
            .WithPageSize(1));

        return await accountRepository
            .QueryAsync(accountQuery, cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public static async Task<Account?> FindAccountByAccountNumberAsync(
        this IAccountRepository accountRepository,
        string accountNumber,
        CancellationToken cancellationToken)
    {
        var accountQuery = AccountQuery.Build(builder => builder
            .WithAccountNumber(accountNumber)
            .WithPageSize(1));

        return await accountRepository
            .QueryAsync(accountQuery, cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);
    }
}