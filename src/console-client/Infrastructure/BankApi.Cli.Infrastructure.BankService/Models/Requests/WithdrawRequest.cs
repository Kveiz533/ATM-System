namespace BankApi.Cli.Infrastructure.BankService.Models.Requests;

public sealed record WithdrawRequest(
    Guid SessionId,
    decimal Money);