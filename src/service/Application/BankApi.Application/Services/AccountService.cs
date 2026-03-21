using BankApi.Application.Abstractions;
using BankApi.Application.Abstractions.Queries;
using BankApi.Application.Contracts.Accounts;
using BankApi.Application.Contracts.Accounts.Operations;
using BankApi.Application.Extensions;
using BankApi.Application.Mapping;
using BankApi.Domain.Accounts;
using BankApi.Domain.Accounts.Results;
using BankApi.Domain.BankOperations;
using BankApi.Domain.Sessions;
using BankApi.Domain.ValueObjects;
using System.Transactions;

namespace BankApi.Application.Services;

public sealed class AccountService : IAccountService
{
    private readonly IPersistenceContext _context;

    public AccountService(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<Balance.Response> BalanceAsync(
        Balance.Request request,
        CancellationToken cancellationToken)
    {
        UserSession? session = await _context.UserSessionRepository.FindUserSessionByIdAsync(request.SessionId, cancellationToken);

        if (session is null)
        {
            return new Balance.Response.Failure("User session not found");
        }

        Account? account = await _context.AccountRepository.FindAccountByIdAsync(session.AccountId, cancellationToken);

        if (account is null)
        {
            return new Balance.Response.Failure("Account not found");
        }

        var bankOperation = new BankOperation(
            BankOperationId.Zero(),
            account.Id,
            BankOperationType.ShowBalance,
            account.Balance,
            DateTimeOffset.UtcNow);

        await _context.OperationHistoryRepository
            .AddAsync([bankOperation], cancellationToken)
            .ToArrayAsync(cancellationToken);

        return new Balance.Response.Success(account.Balance.MapToDto());
    }

    public async Task<Deposit.Response> DepositAsync(
        Deposit.Request request,
        CancellationToken cancellationToken)
    {
        UserSession? session = await _context.UserSessionRepository.FindUserSessionByIdAsync(request.SessionId, cancellationToken);

        if (session is null)
        {
            return new Deposit.Response.Failure("User session not found");
        }

        Account? account = await _context.AccountRepository.FindAccountByIdAsync(session.AccountId, cancellationToken);

        if (account is null)
        {
            return new Deposit.Response.Failure("Account not found");
        }

        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, },
            TransactionScopeAsyncFlowOption.Enabled);

        account.Deposit(new Money(request.Amount));

        await _context.AccountRepository
            .UpdateAsync([account], cancellationToken)
            .ToArrayAsync(cancellationToken);

        var bankOperation = new BankOperation(
            BankOperationId.Zero(),
            account.Id,
            BankOperationType.Deposit,
            account.Balance,
            DateTimeOffset.UtcNow);

        await _context.OperationHistoryRepository
            .AddAsync([bankOperation], cancellationToken)
            .ToArrayAsync(cancellationToken);

        scope.Complete();
        return new Deposit.Response.Success(account.Balance.MapToDto());
    }

    public async Task<Withdraw.Response> WithdrawAsync(
        Withdraw.Request request,
        CancellationToken cancellationToken)
    {
        UserSession? session = await _context.UserSessionRepository.FindUserSessionByIdAsync(request.SessionId, cancellationToken);

        if (session is null)
        {
            return new Withdraw.Response.Failure("User session not found");
        }

        Account? account = await _context.AccountRepository.FindAccountByIdAsync(session.AccountId, cancellationToken);

        if (account is null)
        {
            return new Withdraw.Response.Failure("Account not found");
        }

        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, },
            TransactionScopeAsyncFlowOption.Enabled);

        WithdrawResult withDrawResult = account.Withdraw(new Money(request.Amount));

        if (withDrawResult is WithdrawResult.Failure failure)
        {
            return new Withdraw.Response.Failure(failure.Error);
        }

        await _context.AccountRepository
            .UpdateAsync([account], cancellationToken)
            .ToArrayAsync(cancellationToken);

        var bankOperation = new BankOperation(
            BankOperationId.Zero(),
            account.Id,
            BankOperationType.Withdraw,
            account.Balance,
            DateTimeOffset.UtcNow);

        await _context.OperationHistoryRepository
            .AddAsync([bankOperation], cancellationToken)
            .ToArrayAsync(cancellationToken);

        scope.Complete();
        return new Withdraw.Response.Success(account.Balance.MapToDto());
    }

    public async Task<OperationHistory.Response> OperationHistoryAsync(
        OperationHistory.Request request,
        CancellationToken cancellationToken)
    {
        UserSession? session = await _context.UserSessionRepository.FindUserSessionByIdAsync(request.SessionId, cancellationToken);

        if (session is null)
        {
            return new OperationHistory.Response.Failure("User session not found");
        }

        BankOperationId? cursor = request.PageToken is not null
            ? new BankOperationId(request.PageToken.Value.Key)
            : null;

        var bankOperationQuery = OperationHistoryQuery.Build(builder => builder
            .WithAccountId(session.AccountId)
            .WithKeyCursor(cursor)
            .WithPageSize(request.PageSize));

        BankOperation[] bankOperations = await _context.OperationHistoryRepository
            .QueryAsync(bankOperationQuery, cancellationToken)
            .ToArrayAsync(cancellationToken);

        OperationHistory.PageToken? responsePageToken = bankOperations.Length < request.PageSize
            ? null
            : new OperationHistory.PageToken(bankOperations[^1].Id.Value);

        return new OperationHistory.Response.Success(bankOperations.MapToDto(), responsePageToken);
    }
}