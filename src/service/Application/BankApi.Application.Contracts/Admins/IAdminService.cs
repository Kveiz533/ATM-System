using BankApi.Application.Contracts.Admins.Operations;

namespace BankApi.Application.Contracts.Admins;

public interface IAdminService
{
    Task<CreateAccount.Response> CreateAccountAsync(CreateAccount.Request request, CancellationToken cancellationToken);
}