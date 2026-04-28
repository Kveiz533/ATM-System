using BankApi.Cli.Application.Abstractions.Users;
using BankApi.Cli.Application.Abstractions.Users.Models;
using BankApi.Cli.Application.Abstractions.Users.Operations;
using BankApi.Cli.Infrastructure.BankService.Models.Requests;
using BankApi.Cli.Infrastructure.BankService.Models.Responses;
using BankApi.Cli.Infrastructure.BankService.Options;
using Microsoft.Extensions.Options;
using Refit;

namespace BankApi.Cli.Infrastructure.BankService;

public class UserClient : IUserClient
{
    private readonly IRefitUserClient _client;
    private readonly IOptionsMonitor<BankClientOptions> _options;

    public UserClient(
        IRefitUserClient client,
        IOptionsMonitor<BankClientOptions> options)
    {
        _client = client;
        _options = options;
    }

    public async Task<LogInUserClient.Result> LogInUserAsync(LogInUserClient.Request request, CancellationToken cancellationToken)
    {
        var httpRequest = new LogInUserRequest(request.AccountNumber, request.PinCode);

        IApiResponse<SessionResponse> response = await _client.LogInUserAsync(
            httpRequest,
            cancellationToken);

        if (response.IsSuccessful)
        {
            return new LogInUserClient.Result.Success(response.Content.SessionId);
        }

        string error = response.Error?.Content ?? response.ReasonPhrase ?? "Unknown error";
        return new LogInUserClient.Result.Failure(error);
    }

    public async Task<BalanceClient.Result> BalanceAsync(BalanceClient.Request request, CancellationToken cancellationToken)
    {
        var httpRequest = new BalanceRequest(request.SessionId);

        IApiResponse<BalanceResponse> response = await _client.BalanceAsync(
            httpRequest,
            cancellationToken);

        if (response.IsSuccessful)
        {
            return new BalanceClient.Result.Success(response.Content.Balance);
        }

        string error = response.Error?.Content ?? response.ReasonPhrase ?? "Unknown error";
        return new BalanceClient.Result.Failure(error);
    }

    public async Task<DepositClient.Result> DepositAsync(DepositClient.Request request, CancellationToken cancellationToken)
    {
        var httpRequest = new DepositRequest(request.SessionId, request.Amount);

        IApiResponse<BalanceResponse> response = await _client.DepositAsync(
            httpRequest,
            cancellationToken);

        if (response.IsSuccessful)
        {
            return new DepositClient.Result.Success(response.Content.Balance);
        }

        string error = response.Error?.Content ?? response.ReasonPhrase ?? "Unknown error";
        return new DepositClient.Result.Failure(error);
    }

    public async Task<WithdrawClient.Result> WithdrawAsync(WithdrawClient.Request request, CancellationToken cancellationToken)
    {
        var httpRequest = new WithdrawRequest(request.SessionId, request.Amount);

        IApiResponse<BalanceResponse> response = await _client.WithdrawAsync(
            httpRequest,
            cancellationToken);

        if (response.IsSuccessful)
        {
            return new WithdrawClient.Result.Success(response.Content.Balance);
        }

        string error = response.Error?.Content ?? response.ReasonPhrase ?? "Unknown error";
        return new WithdrawClient.Result.Failure(error);
    }

    public async Task<OperationHistoryClient.Result> OperationHistoryAsync(OperationHistoryClient.Request request, CancellationToken cancellationToken)
    {
        var history = new List<OperationHistoryDto>();

        int pageSize = _options.CurrentValue.PageSize;
        string? pageToken = null;

        do
        {
            var httpRequest = new OperationHistoryRequest(
                request.SessionId,
                pageToken,
                pageSize);

            IApiResponse<OperationHistoryResponse> response = await _client.ShowHistory(
                httpRequest,
                cancellationToken);

            if (response.IsSuccessful is false)
            {
                string error = response.Error?.Content ?? response.ReasonPhrase ?? "Unknown error";
                return new OperationHistoryClient.Result.Failure(error);
            }

            history.AddRange(response.Content.History
                .Select(entry => new OperationHistoryDto(
                    entry.BankOperationId,
                    entry.AccountId,
                    entry.Time,
                    entry.BankOperationType,
                    entry.Balance)));

            pageToken = response.Content.PageToken;
        }
        while (pageToken is not null);

        return new OperationHistoryClient.Result.Success(history);
    }
}