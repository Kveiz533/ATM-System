namespace BankApi.Cli.Application.Abstractions.SessionProviders;

public interface ISessionProvider
{
    Guid? SessionId { get; set; }
}