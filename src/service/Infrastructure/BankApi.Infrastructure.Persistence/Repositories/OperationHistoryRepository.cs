using BankApi.Application.Abstractions.Queries;
using BankApi.Application.Abstractions.Repositories;
using BankApi.Domain.Accounts;
using BankApi.Domain.BankOperations;
using BankApi.Domain.ValueObjects;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace BankApi.Infrastructure.Persistence.Repositories;

public class OperationHistoryRepository : IOperationHistoryRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public OperationHistoryRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async IAsyncEnumerable<BankOperation> AddAsync(
        IReadOnlyCollection<BankOperation> bankOperations,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (bankOperations.Count == 0)
        {
            yield break;
        }

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        const string sql = """
        INSERT INTO operation_history (account_id, time, operation_type, balance)
        SELECT * FROM UNNEST(:account_ids, :times, :operation_types, :balances)
        RETURNING id, account_id, time, operation_type, balance;
        """;

        long[] accountIds = bankOperations.Select(b => b.AccountId.Value).ToArray();
        DateTimeOffset[] times = bankOperations.Select(b => b.Time).ToArray();
        int[] operationTypes = bankOperations.Select(b => (int)b.BankOperationType).ToArray();
        decimal[] balances = bankOperations.Select(b => b.Balance.Value).ToArray();

        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("account_ids", accountIds),
                new NpgsqlParameter("times", times),
                new NpgsqlParameter("operation_types", operationTypes),
                new NpgsqlParameter("balances", balances),
            },
        };

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new BankOperation(
                id: new BankOperationId(reader.GetInt64("id")),
                accountId: new AccountId(reader.GetInt64("account_id")),
                bankOperationType: (BankOperationType)reader.GetInt32("operation_type"),
                newBalance: new Money(reader.GetDecimal("balance")),
                time: reader.GetFieldValue<DateTimeOffset>("time"));
        }
    }

    public async IAsyncEnumerable<BankOperation> QueryAsync(
        OperationHistoryQuery operationHistoryQuery,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        const string sql = """
        SELECT id,
            account_id,
            time,
            operation_type,
            balance
        FROM operation_history
        WHERE (cardinality(:account_ids) = 0 OR account_id = ANY(:account_ids))
        AND (:cursor_id IS NULL OR id < :cursor_id)
        ORDER BY time DESC, id DESC
        LIMIT :page_size;
        """;

        long[] rawIds = operationHistoryQuery.AccountIds.Select(id => id.Value).ToArray();
        long? rowKeyCursor = operationHistoryQuery.KeyCursor?.Value;

        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("account_ids", rawIds),
                new NpgsqlParameter<long?>("cursor_id", rowKeyCursor),
                new NpgsqlParameter("page_size", operationHistoryQuery.PageSize),
            },
        };

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new BankOperation(
                id: new BankOperationId(reader.GetInt64("id")),
                accountId: new AccountId(reader.GetInt64("account_id")),
                bankOperationType: (BankOperationType)reader.GetInt32("operation_type"),
                newBalance: new Money(reader.GetDecimal("balance")),
                time: reader.GetFieldValue<DateTimeOffset>("time"));
        }
    }
}