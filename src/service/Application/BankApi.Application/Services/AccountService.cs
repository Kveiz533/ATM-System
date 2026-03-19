using BankApi.Application.Abstractions;
using BankApi.Application.Abstractions.Queries;
using BankApi.Application.Contracts.Accounts;
using BankApi.Application.Contracts.Accounts.Operations;
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
        var sessionQuery = SessionQuery.Build(builder => builder
            .WithSessionId(request.SessionId)
            .WithPageSize(1));

        UserSession[] sessions = await _context.UserSessionRepository
            .QueryAsync(sessionQuery, cancellationToken)
            .ToArrayAsync(cancellationToken);

        if (sessions.Length == 0)
        {
            return new Balance.Response.Failure("User session not found");
        }

        var accountQuery = AccountQuery.Build(builder => builder
            .WithAccountId(sessions[0].AccountId)
            .WithPageSize(1));

        Account[] accounts = await _context.AccountRepository
            .QueryAsync(accountQuery, cancellationToken)
            .ToArrayAsync(cancellationToken);

        if (accounts.Length == 0)
        {
            return new Balance.Response.Failure("Account not found");
        }

        var bankOperation = new BankOperation(
            BankOperationId.Zero(),
            accounts[0].Id,
            BankOperationType.ShowBalance,
            accounts[0].Balance,
            DateTimeOffset.UtcNow);

        await _context.OperationHistoryRepository
            .AddAsync([bankOperation], cancellationToken)
            .ToArrayAsync(cancellationToken);

        return new Balance.Response.Success(accounts[0].Balance.MapToDto());
    }

    public async Task<Deposit.Response> DepositAsync(
        Deposit.Request request,
        CancellationToken cancellationToken)
    {
        var sessionQuery = SessionQuery.Build(builder => builder
            .WithSessionId(request.SessionId)
            .WithPageSize(1));

        UserSession[] sessions = await _context.UserSessionRepository
            .QueryAsync(sessionQuery, cancellationToken)
            .ToArrayAsync(cancellationToken);

        if (sessions.Length == 0)
        {
            return new Deposit.Response.Failure("User session not found");
        }

        var accountQuery = AccountQuery.Build(builder => builder
            .WithAccountId(sessions[0].AccountId)
            .WithPageSize(1));

        Account[] accounts = await _context.AccountRepository
            .QueryAsync(accountQuery, cancellationToken)
            .ToArrayAsync(cancellationToken);

        if (accounts.Length == 0)
        {
            return new Deposit.Response.Failure("Account not found");
        }

        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, },
            TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            accounts[0].Deposit(new Money(request.Amount));

            await _context.AccountRepository.UpdateAsync([accounts[0]], cancellationToken)
                .ToArrayAsync(cancellationToken);

            var bankOperation = new BankOperation(
                BankOperationId.Zero(),
                accounts[0].Id,
                BankOperationType.Deposit,
                accounts[0].Balance,
                DateTimeOffset.UtcNow);

            await _context.OperationHistoryRepository
                .AddAsync([bankOperation], cancellationToken)
                .ToArrayAsync(cancellationToken);

            scope.Complete();
            return new Deposit.Response.Success(accounts[0].Balance.MapToDto());
        }
        catch (Exception ex)
        {
            return new Deposit.Response.Failure("Transaction failed: " + ex.Message);
        }
    }

    public async Task<Withdraw.Response> WithdrawAsync(
        Withdraw.Request request,
        CancellationToken cancellationToken)
    {
        var sessionQuery = SessionQuery.Build(builder => builder
            .WithSessionId(request.SessionId)
            .WithPageSize(1));

        UserSession[] sessions = await _context.UserSessionRepository
            .QueryAsync(sessionQuery, cancellationToken)
            .ToArrayAsync(cancellationToken);

        if (sessions.Length == 0)
        {
            return new Withdraw.Response.Failure("User session not found");
        }

        var accountQuery = AccountQuery.Build(builder => builder
            .WithAccountId(sessions[0].AccountId)
            .WithPageSize(1));

        Account[] accounts = await _context.AccountRepository
            .QueryAsync(accountQuery, cancellationToken)
            .ToArrayAsync(cancellationToken);

        if (accounts.Length == 0)
        {
            return new Withdraw.Response.Failure("Account not found");
        }

        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, },
            TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            WithdrawResult withDrawResult = accounts[0].Withdraw(new Money(request.Amount));

            if (withDrawResult is WithdrawResult.Failure failure)
            {
                return new Withdraw.Response.Failure(failure.Error);
            }

            await _context.AccountRepository.UpdateAsync([accounts[0]], cancellationToken)
                .ToArrayAsync(cancellationToken);

            var bankOperation = new BankOperation(
                BankOperationId.Zero(),
                accounts[0].Id,
                BankOperationType.Withdraw,
                accounts[0].Balance,
                DateTimeOffset.UtcNow);

            await _context.OperationHistoryRepository
                .AddAsync([bankOperation], cancellationToken)
                .ToArrayAsync(cancellationToken);

            scope.Complete();
            return new Withdraw.Response.Success(accounts[0].Balance.MapToDto());
        }
        catch (Exception ex)
        {
            return new Withdraw.Response.Failure("Transaction failed: " + ex.Message);
        }
    }

    public async Task<OperationHistory.Response> OperationHistoryAsync(
        OperationHistory.Request request,
        CancellationToken cancellationToken)
    {
        var sessionQuery = SessionQuery.Build(builder => builder
            .WithSessionId(request.SessionId)
            .WithPageSize(1));

        UserSession[] sessions = await _context.UserSessionRepository
            .QueryAsync(sessionQuery, cancellationToken)
            .ToArrayAsync(cancellationToken);

        if (sessions.Length == 0)
        {
            return new OperationHistory.Response.Failure("User session not found");
        }

        BankOperationId? cursor = request.PageToken is { } token
            ? new BankOperationId(token.Key)
            : null;

        var bankOperationQuery = OperationHistoryQuery.Build(builder => builder
            .WithAccountId(sessions[0].AccountId)
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