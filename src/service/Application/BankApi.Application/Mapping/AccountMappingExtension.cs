using BankApi.Application.Contracts.Admins.Models;
using BankApi.Domain.Accounts;

namespace BankApi.Application.Mapping;

public static class AccountMappingExtension
{
    public static AccountDto MapToDto(this Account account)
    {
        return new AccountDto(account.AccountNumber);
    }
}