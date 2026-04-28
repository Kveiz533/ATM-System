using BankApi.Application.Abstractions.Queries;
using BankApi.Domain.Sessions;

namespace BankApi.Application.Abstractions.Repositories;

public interface IUserSessionRepository
{
    Task AddAsync(IReadOnlyCollection<UserSession> userSessions, CancellationToken cancellationToken);

    IAsyncEnumerable<UserSession> QueryAsync(SessionQuery sessionQuery, CancellationToken cancellationToken);
}