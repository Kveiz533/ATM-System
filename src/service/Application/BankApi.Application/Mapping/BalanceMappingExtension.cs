using BankApi.Application.Contracts.Accounts.Models;
using BankApi.Domain.ValueObjects;

namespace BankApi.Application.Mapping;

public static class BalanceMappingExtension
{
    public static BalanceDto MapToDto(this Money money)
    {
        return new BalanceDto(money.Value);
    }
}