using BankApi.Cli.Application.Abstractions.Admins;
using BankApi.Cli.Application.Abstractions.Admins.Operations;
using BankApi.Cli.Application.Abstractions.SessionProviders;
using BankApi.Cli.Application.Contracts.Admins;
using BankApi.Cli.Application.Contracts.Admins.Operations;
using System.Diagnostics;

namespace BankApi.Cli.Application.Services;

public sealed class AdminService : IAdminService
{
    private readonly IAdminClient _client;
    private readonly ISessionProvider _sessionProvider;

    public AdminService(IAdminClient client, ISessionProvider sessionProvider)
    {
        _client = client;
        _sessionProvider = sessionProvider;
    }

    public async Task<LogInAdmin.Response> LogInAdminAsync(LogInAdmin.Request request, CancellationToken cancellationToken)
    {
        var clientRequest = new LogInAdminClient.Request(request.SystemPassword);

        LogInAdminClient.Result response = await _client.LogInAdminAsync(clientRequest, cancellationToken);

        if (response is LogInAdminClient.Result.Success success)
        {
            _sessionProvider.SessionId = success.SessionId;
            return new LogInAdmin.Response.Success();
        }

        if (response is LogInAdminClient.Result.Failure failure)
        {
            return new LogInAdmin.Response.Failure(failure.Message);
        }

        throw new UnreachableException();
    }

    public async Task<CreateAccount.Response> CreateAccountAsync(CreateAccount.Request request, CancellationToken cancellationToken)
    {
        if (_sessionProvider.SessionId is not { } sessionId)
            return new CreateAccount.Response.Failure("Not authorized");

        var clientRequest = new CreateAccountClient.Request(sessionId, request.AccountNumber, request.PinCode);

        CreateAccountClient.Result response = await _client.CreateAccountAsync(clientRequest, cancellationToken);

        if (response is CreateAccountClient.Result.Success)
        {
            return new CreateAccount.Response.Success();
        }

        if (response is CreateAccountClient.Result.Failure failure)
        {
            return new CreateAccount.Response.Failure(failure.Message);
        }

        throw new UnreachableException();
    }
}