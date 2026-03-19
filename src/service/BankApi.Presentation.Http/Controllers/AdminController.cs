using BankApi.Application.Contracts.Admins;
using BankApi.Application.Contracts.Admins.Models;
using BankApi.Application.Contracts.Admins.Operations;
using BankApi.Presentation.Http.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BankApi.Presentation.Http.Controllers;

[ApiController]
[Route("/api/admin")]
public sealed class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost("accounts")]
    public async Task<ActionResult<AccountDto>> CreateAccountAsync(
        CreateAccountRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var request = new CreateAccount.Request(
            httpRequest.SessionId,
            httpRequest.AccountNumber,
            httpRequest.PinCode);

        CreateAccount.Response response = await _adminService.CreateAccountAsync(request, cancellationToken);
        return response switch
        {
            CreateAccount.Response.Success success => Ok(success.AccountNumber),
            CreateAccount.Response.Failure failure => BadRequest(failure.Message),
            _ => throw new UnreachableException(),
        };
    }
}