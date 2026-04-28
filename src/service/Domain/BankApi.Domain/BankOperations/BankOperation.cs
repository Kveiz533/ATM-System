using BankApi.Domain.Accounts;
using BankApi.Domain.ValueObjects;

namespace BankApi.Domain.BankOperations;

public sealed record BankOperation
{
    public BankOperation(BankOperationId id, AccountId accountId, BankOperationType bankOperationType, Money newBalance, DateTimeOffset time)
    {
        Id = id;
        AccountId = accountId;
        BankOperationType = bankOperationType;
        Balance = newBalance;
        Time = time;
    }

    public BankOperationId Id { get; }

    public AccountId AccountId { get; }

    public DateTimeOffset Time { get; }

    public BankOperationType BankOperationType { get; }

    public Money Balance { get; }

    public override string ToString()
    {
        return $"Operation {Id.Value}: {BankOperationType} on account {AccountId.Value}, New balance: {Balance.Value} at {Time}";
    }
}