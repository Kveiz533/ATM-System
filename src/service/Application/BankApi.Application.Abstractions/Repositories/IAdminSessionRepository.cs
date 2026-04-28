using BankApi.Application.Abstractions.Queries;
using BankApi.Domain.Sessions;

namespace BankApi.Application.Abstractions.Repositories;

public interface IAdminSessionRepository
{
    Task AddAsync(IReadOnlyCollection<AdminSession> adminSessions, CancellationToken cancellationToken);

    IAsyncEnumerable<AdminSession> QueryAsync(SessionQuery sessionQuery, CancellationToken cancellationToken);
}