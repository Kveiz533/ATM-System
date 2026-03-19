using BankApi.Application.Abstractions.Queries;
using BankApi.Application.Abstractions.Repositories;
using BankApi.Domain.Accounts;
using BankApi.Domain.ValueObjects;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace BankApi.Infrastructure.Persistence.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public AccountRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async IAsyncEnumerable<Account> AddAsync(
        IReadOnlyCollection<Account> accounts,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        const string sql = """
                           INSERT INTO accounts (account_number, pin_code, balance)
                           VALUES (:account_number, :pin_code, :balance)
                           RETURNING id, account_number, pin_code, balance;
                           """;

        foreach (Account account in accounts)
        {
            await using var command = new NpgsqlCommand(sql, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("account_number", account.AccountNumber),
                    new NpgsqlParameter("pin_code", account.PinCode),
                    new NpgsqlParameter("balance", account.Balance.Value),
                },
            };

            await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

            if (await reader.ReadAsync(cancellationToken))
            {
                yield return new Account(
                    id: new AccountId(reader.GetInt64("id")),
                    accountNumber: reader.GetString("account_number"),
                    pinCode: reader.GetString("pin_code"),
                    balance: new Money(reader.GetDecimal("balance")));
            }
        }
    }

    public async IAsyncEnumerable<Account> UpdateAsync(
        IReadOnlyCollection<Account> accounts,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        const string sql = """
                           UPDATE accounts 
                           SET pin_code = :pin_code, 
                               balance = :balance
                           WHERE id = :id
                           RETURNING id, account_number, pin_code, balance;
                           """;

        foreach (Account account in accounts)
        {
            await using var command = new NpgsqlCommand(sql, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("id", account.Id.Value),
                    new NpgsqlParameter("pin_code", account.PinCode),
                    new NpgsqlParameter("balance", account.Balance.Value),
                },
            };

            await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

            if (await reader.ReadAsync(cancellationToken))
            {
                yield return new Account(
                    id: new AccountId(reader.GetInt64("id")),
                    accountNumber: reader.GetString("account_number"),
                    pinCode: reader.GetString("pin_code"),
                    balance: new Money(reader.GetDecimal("balance")));
            }
        }
    }

    public async IAsyncEnumerable<Account> QueryAsync(
        AccountQuery accountQuery,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        const string sql = """
                           SELECT id,
                                  account_number,
                                  pin_code,
                                  balance
                           FROM accounts
                           WHERE (cardinality(:account_ids) = 0 OR id = ANY(:account_ids))
                             AND (cardinality(:account_numbers) = 0 OR account_number = ANY(:account_numbers))
                             AND (:cursor_id IS NULL OR id > :cursor_id)
                           ORDER BY id
                           LIMIT :page_size;
                           """;

        long[] rawIds = accountQuery.AccountIds.Select(id => id.Value).ToArray();
        long? rowKeyCursor = accountQuery.KeyCursor?.Value;

        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("account_ids", rawIds),
                new NpgsqlParameter("account_numbers", accountQuery.AccountNumbers),
                new NpgsqlParameter<long?>("cursor_id", rowKeyCursor),
                new NpgsqlParameter("page_size", accountQuery.PageSize),
            },
        };

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Account(
                id: new AccountId(reader.GetInt64("id")),
                accountNumber: reader.GetString("account_number"),
                pinCode: reader.GetString("pin_code"),
                balance: new Money(reader.GetDecimal("balance")));
        }
    }
}