namespace BankApi.Domain.Accounts;

public sealed record AccountId
{
    public long Value { get; }

    public AccountId(long value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "AccountId cannot be negative");
        }

        Value = value;
    }

    public static AccountId Zero()
    {
        return new AccountId(0);
    }
}