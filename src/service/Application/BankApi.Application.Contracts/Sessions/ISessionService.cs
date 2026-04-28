using BankApi.Application.Contracts.Sessions.Operations;

namespace BankApi.Application.Contracts.Sessions;

public interface ISessionService
{
    Task<LogInUser.Response> LogInUserAsync(LogInUser.Request request, CancellationToken cancellationToken);

    Task<LogInAdmin.Response> LogInAdminAsync(LogInAdmin.Request request, CancellationToken cancellationToken);
}