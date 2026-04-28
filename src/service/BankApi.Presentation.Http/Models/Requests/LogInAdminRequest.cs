using System.ComponentModel.DataAnnotations;

namespace BankApi.Presentation.Http.Models.Requests;

public sealed class LogInAdminRequest
{
    [MinLength(4, ErrorMessage = "System password must be at least 4 characters long.")]
    public required string SystemPassword { get; init; }
}