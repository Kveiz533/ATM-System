using BankApi.Cli.Application.Abstractions.Admins.Operations;

namespace BankApi.Cli.Application.Abstractions.Admins;

public interface IAdminClient
{
    Task<LogInAdminClient.Result> LogInAdminAsync(
        LogInAdminClient.Request request,
        CancellationToken cancellationToken);

    Task<CreateAccountClient.Result> CreateAccountAsync(
        CreateAccountClient.Request request,
        CancellationToken cancellationToken);
}