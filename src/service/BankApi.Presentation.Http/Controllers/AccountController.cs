using BankApi.Application.Contracts.Accounts;
using BankApi.Application.Contracts.Accounts.Models;
using BankApi.Application.Contracts.Accounts.Operations;
using BankApi.Presentation.Http.Models.Requests;
using BankApi.Presentation.Http.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace BankApi.Presentation.Http.Controllers;

[ApiController]
[Route("/api/accounts")]
public sealed class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("balance")]
    public async Task<ActionResult<BalanceDto>> BalanceAsync(
        [FromQuery] BalanceRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var request = new Balance.Request(httpRequest.SessionId);
        Balance.Response response = await _accountService.BalanceAsync(request, cancellationToken);
        return response switch
        {
            Balance.Response.Success success => Ok(success.Balance),
            Balance.Response.Failure failure => BadRequest(failure.Message),
            _ => throw new UnreachableException(),
        };
    }

    [HttpPost("deposits")]
    public async Task<ActionResult<BalanceDto>> DepositAsync(
        [FromBody] DepositRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var request = new Deposit.Request(httpRequest.SessionId, httpRequest.Money);
        Deposit.Response response = await _accountService.DepositAsync(request, cancellationToken);
        return response switch
        {
            Deposit.Response.Success success => Ok(success.Balance),
            Deposit.Response.Failure failure => BadRequest(failure.Message),
            _ => throw new UnreachableException(),
        };
    }

    [HttpPost("withdrawals")]
    public async Task<ActionResult<BalanceDto>> WithdrawAsync(
        [FromBody] WithdrawRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var request = new Withdraw.Request(httpRequest.SessionId, httpRequest.Money);
        Withdraw.Response response = await _accountService.WithdrawAsync(request, cancellationToken);
        return response switch
        {
            Withdraw.Response.Success success => Ok(success.Balance),
            Withdraw.Response.Failure failure => BadRequest(failure.Message),
            _ => throw new UnreachableException(),
        };
    }

    [HttpGet("history")]
    public async Task<ActionResult<OperationHistoryResponse>> ShowHistoryAsync(
        [FromQuery] OperationHistoryRequest httpRequest,
        CancellationToken cancellationToken)
    {
        OperationHistory.PageToken? pageToken = httpRequest.PageToken is null
            ? null
            : JsonSerializer.Deserialize<OperationHistory.PageToken>(httpRequest.PageToken);

        var request = new OperationHistory.Request(
            httpRequest.SessionId,
            httpRequest.PageSize,
            pageToken);

        OperationHistory.Response response = await _accountService.OperationHistoryAsync(
            request,
            cancellationToken);

        if (response is OperationHistory.Response.Failure operationHistoryFailure)
        {
            return BadRequest(operationHistoryFailure.Message);
        }

        if (response is OperationHistory.Response.Success operationHistorySuccess)
        {
            string? responsePageToken = operationHistorySuccess.PageToken is null
                ? null
                : JsonSerializer.Serialize(operationHistorySuccess.PageToken.Value);

            return Ok(new OperationHistoryResponse
            {
                History = operationHistorySuccess.History,
                PageToken = responsePageToken,
            });
        }

        throw new UnreachableException();
    }
}