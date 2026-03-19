using BankApi.Cli.Application.Abstractions.SessionProviders;

namespace BankApi.Cli.Application.Providers;

public class SessionProvider : ISessionProvider
{
    public Guid? SessionId { get; set; }
}