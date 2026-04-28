using BankApi.Cli.Application.Contracts.Admins.Operations;

namespace BankApi.Cli.Application.Contracts.Admins;

public interface IAdminService
{
    Task<LogInAdmin.Response> LogInAdminAsync(
        LogInAdmin.Request request,
        CancellationToken cancellationToken);

    Task<CreateAccount.Response> CreateAccountAsync(
        CreateAccount.Request request,
        CancellationToken cancellationToken);
}