using System.ComponentModel.DataAnnotations;

namespace BankApi.Cli.Infrastructure.BankService.Options;

public sealed class BankClientOptions
{
    public required Uri Address { get; init; }

    [Range(minimum: 1, maximum: 100)]
    public int PageSize { get; init; }
}