using BankApi.Domain.Accounts.Results;
using BankApi.Domain.ValueObjects;

namespace BankApi.Domain.Accounts;

public class Account
{
    public AccountId Id { get; }

    public string PinCode { get; }

    public string AccountNumber { get; }

    public Money Balance { get; private set; }

    public Account(AccountId id, string accountNumber, string pinCode, Money balance)
    {
        Id = id;
        AccountNumber = accountNumber;
        PinCode = pinCode;
        Balance = balance;
    }

    public bool VerifyPinCode(string pinCode)
    {
        return PinCode == pinCode;
    }

    public void Deposit(Money amount)
    {
        Balance += amount;
    }

    public WithdrawResult Withdraw(Money amount)
    {
        if (amount > Balance)
        {
            return new WithdrawResult.Failure("Insufficient funds");
        }

        Balance -= amount;
        return new WithdrawResult.Success();
    }
}