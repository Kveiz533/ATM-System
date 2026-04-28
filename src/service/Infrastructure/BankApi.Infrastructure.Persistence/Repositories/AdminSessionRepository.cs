using BankApi.Application.Abstractions.Queries;
using BankApi.Application.Abstractions.Repositories;
using BankApi.Domain.Sessions;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace BankApi.Infrastructure.Persistence.Repositories;

public class AdminSessionRepository : IAdminSessionRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public AdminSessionRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task AddAsync(IReadOnlyCollection<AdminSession> adminSessions, CancellationToken cancellationToken)
    {
        if (adminSessions.Count == 0)
        {
            return;
        }

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        const string sql = """
        INSERT INTO admin_sessions (id)
        SELECT * FROM UNNEST(:ids)
        """;

        Guid[] ids = adminSessions.Select(a => a.SessionId).ToArray();

        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("ids", ids),
            },
        };

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async IAsyncEnumerable<AdminSession> QueryAsync(
        SessionQuery sessionQuery,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        const string sql = """
        SELECT id
        FROM admin_sessions
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
            yield return new AdminSession(sessionId: reader.GetGuid("id"));
        }
    }
}