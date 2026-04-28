using BankApi.Cli.Application.Abstractions.Users.Operations;

namespace BankApi.Cli.Application.Abstractions.Users;

public interface IUserClient
{
    Task<LogInUserClient.Result> LogInUserAsync(
        LogInUserClient.Request request,
        CancellationToken cancellationToken);

    Task<BalanceClient.Result> BalanceAsync(
        BalanceClient.Request request,
        CancellationToken cancellationToken);

    Task<DepositClient.Result> DepositAsync(
        DepositClient.Request request,
        CancellationToken cancellationToken);

    Task<WithdrawClient.Result> WithdrawAsync(
        WithdrawClient.Request request,
        CancellationToken cancellationToken);

    Task<OperationHistoryClient.Result> OperationHistoryAsync(
        OperationHistoryClient.Request request,
        CancellationToken cancellationToken);
}