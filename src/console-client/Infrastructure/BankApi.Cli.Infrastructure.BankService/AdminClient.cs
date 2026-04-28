using BankApi.Cli.Application.Abstractions.Admins;
using BankApi.Cli.Application.Abstractions.Admins.Operations;
using BankApi.Cli.Infrastructure.BankService.Models.Requests;
using BankApi.Cli.Infrastructure.BankService.Models.Responses;
using Refit;

namespace BankApi.Cli.Infrastructure.BankService;

public class AdminClient : IAdminClient
{
    private readonly IRefitAdminClient _client;

    public AdminClient(IRefitAdminClient client)
    {
        _client = client;
    }

    public async Task<LogInAdminClient.Result> LogInAdminAsync(LogInAdminClient.Request request, CancellationToken cancellationToken)
    {
        var httpRequest = new LogInAdminRequest(request.SystemPassword);

        IApiResponse<SessionResponse> response = await _client.LogInAdminAsync(
            httpRequest,
            cancellationToken);

        if (response.IsSuccessful)
        {
            return new LogInAdminClient.Result.Success(response.Content.SessionId);
        }

        string error = response.Error?.Content ?? response.ReasonPhrase ?? "Unknown error";
        return new LogInAdminClient.Result.Failure(error);
    }

    public async Task<CreateAccountClient.Result> CreateAccountAsync(CreateAccountClient.Request request, CancellationToken cancellationToken)
    {
        var httpRequest = new CreateAccountRequest(
            request.SessionId,
            request.AccountNumber,
            request.PinCode);

        IApiResponse<AccountResponse> response = await _client.CreateAccountAsync(
            httpRequest,
            cancellationToken);

        if (response.IsSuccessful)
        {
            return new CreateAccountClient.Result.Success();
        }

        string error = response.Error?.Content ?? response.ReasonPhrase ?? "Unknown error";
        return new CreateAccountClient.Result.Failure(error);
    }
}