using BankApi.Application.Abstractions.Queries;
using BankApi.Application.Abstractions.Repositories;
using BankApi.Domain.Sessions;

namespace BankApi.Application.Extensions;

public static class UserSessionRepositoryQueryExtensions
{
    public static async Task<UserSession?> FindUserSessionByIdAsync(
        this IUserSessionRepository userSessionRepository,
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        var sessionQuery = SessionQuery.Build(builder => builder
            .WithSessionId(sessionId)
            .WithPageSize(1));

        return await userSessionRepository
            .QueryAsync(sessionQuery, cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);
    }
}