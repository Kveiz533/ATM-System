using BankApi.Cli.Application.Contracts.Users.Operations;

namespace BankApi.Cli.Application.Contracts.Users;

public interface IUserService
{
    Task<LogInUser.Response> LogInUserAsync(
        LogInUser.Request request,
        CancellationToken cancellationToken);

    Task<Balance.Response> BalanceAsync(
        CancellationToken cancellationToken);

    Task<Deposit.Response> DepositAsync(
        Deposit.Request request,
        CancellationToken cancellationToken);

    Task<Withdraw.Response> WithdrawAsync(
        Withdraw.Request request,
        CancellationToken cancellationToken);

    Task<OperationHistory.Response> OperationHistoryAsync(
        CancellationToken cancellationToken);
}