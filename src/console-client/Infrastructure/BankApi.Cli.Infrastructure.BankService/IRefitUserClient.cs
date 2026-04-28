using BankApi.Cli.Infrastructure.BankService.Models.Requests;
using BankApi.Cli.Infrastructure.BankService.Models.Responses;
using Refit;

namespace BankApi.Cli.Infrastructure.BankService;

public interface IRefitUserClient
{
    [Post("/api/sessions/user")]
    Task<IApiResponse<SessionResponse>> LogInUserAsync(
        [Body] LogInUserRequest request,
        CancellationToken cancellationToken);

    [Get("/api/accounts/balance")]
    Task<IApiResponse<BalanceResponse>> BalanceAsync(
        [Query] BalanceRequest request,
        CancellationToken cancellationToken);

    [Post("/api/accounts/deposits")]
    Task<IApiResponse<BalanceResponse>> DepositAsync(
        [Body] DepositRequest request,
        CancellationToken cancellationToken);

    [Post("/api/accounts/withdrawals")]
    Task<IApiResponse<BalanceResponse>> WithdrawAsync(
        [Body] WithdrawRequest request,
        CancellationToken cancellationToken);

    [Get("/api/accounts/history")]
    Task<IApiResponse<OperationHistoryResponse>> ShowHistory(
        [Query] OperationHistoryRequest request,
        CancellationToken cancellationToken);
}