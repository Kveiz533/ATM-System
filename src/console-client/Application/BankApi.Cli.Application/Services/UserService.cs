using BankApi.Cli.Application.Abstractions.Users;
using BankApi.Cli.Application.Abstractions.Users.Operations;
using BankApi.Cli.Application.Contracts.Users;
using BankApi.Cli.Application.Contracts.Users.Operations;
using BankApi.Cli.Application.Mapping;
using BankApi.Cli.Application.Providers;
using System.Diagnostics;

namespace BankApi.Cli.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IUserClient _client;
    private readonly SessionProvider _sessionProvider;

    public UserService(IUserClient client, SessionProvider sessionProvider)
    {
        _client = client;
        _sessionProvider = sessionProvider;
    }

    public async Task<LogInUser.Response> LogInUserAsync(LogInUser.Request request, CancellationToken cancellationToken)
    {
        var clientRequest = new LogInUserClient.Request(
            request.AccountNumber,
            request.PinCode);

        LogInUserClient.Result response = await _client.LogInUserAsync(clientRequest, cancellationToken);

        if (response is LogInUserClient.Result.Success success)
        {
            _sessionProvider.SessionId = success.SessionId;
            return new LogInUser.Response.Success();
        }

        if (response is LogInUserClient.Result.Failure failure)
        {
            return new LogInUser.Response.Failure(failure.Message);
        }

        throw new UnreachableException();
    }

    public async Task<Balance.Response> BalanceAsync(CancellationToken cancellationToken)
    {
        if (_sessionProvider.SessionId is not { } sessionId)
            return new Balance.Response.Failure("Not authorized");

        var clientRequest = new BalanceClient.Request(sessionId);

        BalanceClient.Result response = await _client.BalanceAsync(clientRequest, cancellationToken);

        if (response is BalanceClient.Result.Success success)
        {
            return new Balance.Response.Success(success.Balance);
        }

        if (response is BalanceClient.Result.Failure failure)
        {
            return new Balance.Response.Failure(failure.Message);
        }

        throw new UnreachableException();
    }

    public async Task<Deposit.Response> DepositAsync(Deposit.Request request, CancellationToken cancellationToken)
    {
        if (_sessionProvider.SessionId is not { } sessionId)
            return new Deposit.Response.Failure("Not authorized");

        var clientRequest = new DepositClient.Request(sessionId, request.Amount);

        DepositClient.Result response = await _client.DepositAsync(clientRequest, cancellationToken);

        if (response is DepositClient.Result.Success success)
        {
            return new Deposit.Response.Success(success.Balance);
        }

        if (response is DepositClient.Result.Failure failure)
        {
            return new Deposit.Response.Failure(failure.Message);
        }

        throw new UnreachableException();
    }

    public async Task<Withdraw.Response> WithdrawAsync(Withdraw.Request request, CancellationToken cancellationToken)
    {
        if (_sessionProvider.SessionId is not { } sessionId)
            return new Withdraw.Response.Failure("Not authorized");

        var clientRequest = new WithdrawClient.Request(sessionId, request.Amount);

        WithdrawClient.Result response = await _client.WithdrawAsync(clientRequest, cancellationToken);

        if (response is WithdrawClient.Result.Success success)
        {
            return new Withdraw.Response.Success(success.Balance);
        }

        if (response is WithdrawClient.Result.Failure failure)
        {
            return new Withdraw.Response.Failure(failure.Message);
        }

        throw new UnreachableException();
    }

    public async Task<OperationHistory.Response> OperationHistoryAsync(CancellationToken cancellationToken)
    {
        if (_sessionProvider.SessionId is not { } sessionId)
            return new OperationHistory.Response.Failure("Not authorized");

        var clientRequest = new OperationHistoryClient.Request(sessionId);

        OperationHistoryClient.Result response = await _client.OperationHistoryAsync(clientRequest, cancellationToken);

        if (response is OperationHistoryClient.Result.Success success)
        {
            return new OperationHistory.Response.Success(success.History.MapToEntity());
        }

        if (response is OperationHistoryClient.Result.Failure failure)
        {
            return new OperationHistory.Response.Failure(failure.Message);
        }

        throw new UnreachableException();
    }
}