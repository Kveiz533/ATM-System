using System.ComponentModel.DataAnnotations;

namespace BankApi.Application.Options;

public class AdminOptions
{
    [MinLength(4, ErrorMessage = "System password must be at least 4 characters long.")]
    public required string SystemPassword { get; init; }
}