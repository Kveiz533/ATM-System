using BankApi.Cli.Infrastructure.BankService.Models.Requests;
using BankApi.Cli.Infrastructure.BankService.Models.Responses;
using Refit;

namespace BankApi.Cli.Infrastructure.BankService;

public interface IRefitAdminClient
{
    [Post("/api/sessions/admin")]
    Task<IApiResponse<SessionResponse>> LogInAdminAsync(
        [Body] LogInAdminRequest request,
        CancellationToken cancellationToken);

    [Post("/api/admin/accounts")]
    Task<IApiResponse<AccountResponse>> CreateAccountAsync(
        [Body] CreateAccountRequest request,
        CancellationToken cancellationToken);
}