using BankApi.Application.Abstractions.Queries;
using BankApi.Application.Abstractions.Repositories;
using BankApi.Domain.Sessions;

namespace BankApi.Application.Extensions;

public static class AdminSessionRepositoryQueryExtensions
{
    public static async Task<AdminSession?> FindAdminSessionByIdAsync(
        this IAdminSessionRepository adminSessionRepository,
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        var sessionQuery = SessionQuery.Build(builder => builder
            .WithSessionId(sessionId)
            .WithPageSize(1));

        return await adminSessionRepository
            .QueryAsync(sessionQuery, cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);
    }
}