namespace BankApi.Cli.Infrastructure.BankService.Models.Requests;

public sealed record LogInUserRequest(
    string AccountNumber,
    string PinCode);