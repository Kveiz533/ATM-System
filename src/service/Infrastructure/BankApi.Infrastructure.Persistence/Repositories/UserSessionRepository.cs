using BankApi.Application.Abstractions.Queries;
using BankApi.Application.Abstractions.Repositories;
using BankApi.Domain.Accounts;
using BankApi.Domain.Sessions;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace BankApi.Infrastructure.Persistence.Repositories;

public class UserSessionRepository : IUserSessionRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public UserSessionRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task AddAsync(IReadOnlyCollection<UserSession> userSessions, CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        const string sql = """
                           INSERT INTO user_sessions (id, account_id)
                           VALUES (:id, :account_id)
                           """;

        foreach (UserSession userSession in userSessions)
        {
            await using var command = new NpgsqlCommand(sql, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("id", userSession.SessionId),
                    new NpgsqlParameter("account_id", userSession.AccountId.Value),
                },
            };

            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    public async IAsyncEnumerable<UserSession> QueryAsync(
        SessionQuery sessionQuery,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        const string sql = """
                           SELECT id,
                                  account_id
                           FROM user_sessions
                           WHERE ((cardinality(:session_ids) = 0 OR id = ANY(:session_ids))
                               AND (:cursor_id IS NULL OR id > :cursor_id))
                           ORDER BY id
                           LIMIT :page_size;
                           """;

        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("session_ids", sessionQuery.SessionIds),
                new NpgsqlParameter<Guid?>("cursor_id", sessionQuery.KeyCursor),
                new NpgsqlParameter("page_size", sessionQuery.PageSize),
            },
        };

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new UserSession(
                sessionId: reader.GetGuid("id"),
                accountId: new AccountId(reader.GetInt64("account_id")));
        }
    }
}