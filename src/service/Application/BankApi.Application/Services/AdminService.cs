using BankApi.Application.Abstractions;
using BankApi.Application.Contracts.Admins;
using BankApi.Application.Contracts.Admins.Operations;
using BankApi.Application.Extensions;
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
        AdminSession? session = await _context.AdminSessionRepository.FindAdminSessionByIdAsync(request.SessionId, cancellationToken);

        if (session is null)
        {
            return new CreateAccount.Response.Failure("Admin session not found");
        }

        Account? account = await _context.AccountRepository.FindAccountByAccountNumberAsync(request.AccountNumber, cancellationToken);

        if (account is not null)
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