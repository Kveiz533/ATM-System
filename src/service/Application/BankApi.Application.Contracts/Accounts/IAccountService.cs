using BankApi.Application.Contracts.Accounts.Operations;

namespace BankApi.Application.Contracts.Accounts;

public interface IAccountService
{
    Task<Balance.Response> BalanceAsync(
        Balance.Request request,
        CancellationToken cancellationToken);

    Task<Deposit.Response> DepositAsync(
        Deposit.Request request,
        CancellationToken cancellationToken);

    Task<Withdraw.Response> WithdrawAsync(
        Withdraw.Request request,
        CancellationToken cancellationToken);

    Task<OperationHistory.Response> OperationHistoryAsync(
        OperationHistory.Request request,
        CancellationToken cancellationToken);
}