using BankApi.Application.Abstractions;
using BankApi.Application.Abstractions.Queries;
using BankApi.Application.Contracts.Admins;
using BankApi.Application.Contracts.Admins.Operations;
using BankApi.Application.Mapping;
using BankApi.Domain.Accounts;
using BankApi.Domain.Sessions;
using BankApi.Domain.ValueObjects;

namespace BankApi.Application.Services;

public sealed class AdminService : IAdminService
{
    private readonly IPersistenceContext _context;

    public AdminService(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<CreateAccount.Response> CreateAccountAsync(
        CreateAccount.Request request,
        CancellationToken cancellationToken)
    {
        var sessionQuery = SessionQuery.Build(builder => builder
            .WithSessionId(request.SessionId)
            .WithPageSize(1));

        AdminSession[] session = await _context.AdminSessionRepository
            .QueryAsync(sessionQuery, cancellationToken)
            .ToArrayAsync(cancellationToken);

        if (session.Length == 0)
        {
            return new CreateAccount.Response.Failure("Admin session not found");
        }

        var accountQuery = AccountQuery.Build(builder => builder
            .WithAccountNumber(request.AccountNumber)
            .WithPageSize(1));

        Account[] existingAccounts = await _context.AccountRepository
            .QueryAsync(accountQuery, cancellationToken)
            .ToArrayAsync(cancellationToken);

        if (existingAccounts.Length > 0)
        {
            return new CreateAccount.Response.Failure("Account with this number already exists");
        }

        var newAccount = new Account(AccountId.Zero(), request.AccountNumber, request.PinCode, Money.Zero());

        Account createdAccount = await _context.AccountRepository
            .AddAsync([newAccount], cancellationToken)
            .FirstAsync(cancellationToken);

        return new CreateAccount.Response.Success(createdAccount.MapToDto());
    }
}