using BankApi.Application.Contracts.Sessions;
using BankApi.Application.Contracts.Sessions.Models;
using BankApi.Application.Contracts.Sessions.Operations;
using BankApi.Presentation.Http.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BankApi.Presentation.Http.Controllers;

[ApiController]
[Route("/api/sessions")]
public sealed class SessionController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SessionController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpPost("admin")]
    public async Task<ActionResult<SessionDto>> LogInAdminAsync(
        [FromBody] LogInAdminRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var request = new LogInAdmin.Request(httpRequest.SystemPassword);
        LogInAdmin.Response response = await _sessionService.LogInAdminAsync(request, cancellationToken);

        return response switch
        {
            LogInAdmin.Response.Success success => Ok(success.Session),
            LogInAdmin.Response.Failure failure => Unauthorized(failure.Message),
            _ => throw new UnreachableException(),
        };
    }

    [HttpPost("user")]
    public async Task<ActionResult<SessionDto>> LogInUserAsync(
        [FromBody] LogInUserRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var request = new LogInUser.Request(httpRequest.AccountNumber, httpRequest.PinCode);
        LogInUser.Response response = await _sessionService.LogInUserAsync(request, cancellationToken);

        return response switch
        {
            LogInUser.Response.Success success => Ok(success.Session),
            LogInUser.Response.Failure failure => Unauthorized(failure.Message),
            _ => throw new UnreachableException(),
        };
    }
}