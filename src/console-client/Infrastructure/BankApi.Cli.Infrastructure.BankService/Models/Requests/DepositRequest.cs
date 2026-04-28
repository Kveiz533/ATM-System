namespace BankApi.Cli.Infrastructure.BankService.Models.Requests;

public sealed record DepositRequest(
    Guid SessionId,
    decimal Money);