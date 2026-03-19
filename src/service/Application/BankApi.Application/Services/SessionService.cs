using BankApi.Application.Abstractions;
using BankApi.Application.Abstractions.Queries;
using BankApi.Application.Contracts.Sessions;
using BankApi.Application.Contracts.Sessions.Operations;
using BankApi.Application.Mapping;
using BankApi.Application.Options;
using BankApi.Domain.Accounts;
using BankApi.Domain.Sessions;
using Microsoft.Extensions.Options;

namespace BankApi.Application.Services;

public sealed class SessionService : ISessionService
{
    private readonly IPersistenceContext _context;
    private readonly string _systemPassword;

    public SessionService(IPersistenceContext context, IOptions<AdminOptions> options)
    {
        _context = context;
        _systemPassword = options.Value.SystemPassword;
    }

    public async Task<LogInUser.Response> LogInUserAsync(
        LogInUser.Request request,
        CancellationToken cancellationToken)
    {
        var accountQuery = AccountQuery.Build(builder => builder
            .WithAccountNumber(request.AccountNumber)
            .WithPageSize(1));

        Account[] accounts = await _context.AccountRepository
            .QueryAsync(accountQuery, cancellationToken)
            .ToArrayAsync(cancellationToken);

        if (accounts.Length == 0)
        {
            return new LogInUser.Response.Failure("No accounts found");
        }

        if (!accounts[0].VerifyPinCode(request.PinCode))
        {
            return new LogInUser.Response.Failure("Invalid pin code");
        }

        var session = new UserSession(Guid.NewGuid(), accounts[0].Id);
        await _context.UserSessionRepository.AddAsync([session], cancellationToken);

        return new LogInUser.Response.Success(session.MapToDto());
    }

    public async Task<LogInAdmin.Response> LogInAdminAsync(
        LogInAdmin.Request request,
        CancellationToken cancellationToken)
    {
        if (request.SystemPassword != _systemPassword)
        {
            return new LogInAdmin.Response.Failure("Wrong system password");
        }

        var session = new AdminSession(Guid.NewGuid());
        await _context.AdminSessionRepository.AddAsync([session], cancellationToken);

        return new LogInAdmin.Response.Success(session.MapToDto());
    }
}