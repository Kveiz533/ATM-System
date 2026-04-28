namespace BankApi.Domain.ValueObjects;

public sealed record Money
{
    public Money(decimal value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Money value cannot be negative");
        }

        Value = value;
    }

    public decimal Value { get; }

    public static Money Zero()
    {
        return new Money(0);
    }

    public static Money operator +(Money left, Money right) => new(left.Value + right.Value);

    public static Money operator -(Money left, Money right) => new(left.Value - right.Value);

    public static bool operator <(Money left, Money right) => left.Value < right.Value;

    public static bool operator >(Money left, Money right) => left.Value > right.Value;
}