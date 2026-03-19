namespace BankApi.Domain.BankOperations;

public sealed record BankOperationId
{
    public long Value { get; }

    public BankOperationId(long value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "AccountId cannot be negative");
        }

        Value = value;
    }

    public static BankOperationId Zero()
    {
        return new BankOperationId(0);
    }
}