namespace BankApi.Cli.Infrastructure.BankService.Models.Requests;

public sealed record CreateAccountRequest(
    Guid SessionId,
    string AccountNumber,
    string PinCode);