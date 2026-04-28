using BankApi.Domain.Accounts;

namespace BankApi.Domain.Sessions;

public sealed class UserSession
{
    public UserSession(Guid sessionId, AccountId accountId)
    {
        SessionId = sessionId;
        AccountId = accountId;
    }

    public Guid SessionId { get; }

    public AccountId AccountId { get; }
}
