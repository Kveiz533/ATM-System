namespace BankApi.Domain.Sessions;

public sealed class AdminSession
{
    public AdminSession(Guid sessionId)
    {
        SessionId = sessionId;
    }

    public Guid SessionId { get; }
}