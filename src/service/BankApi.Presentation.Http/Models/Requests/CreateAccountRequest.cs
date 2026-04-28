using System.ComponentModel.DataAnnotations;

namespace BankApi.Presentation.Http.Models.Requests;

public sealed class CreateAccountRequest
{
    public required Guid SessionId { get; init; }

    [RegularExpression(@"^\d{20}$", ErrorMessage = "Account number must be exactly 20 digits")]
    public required string AccountNumber { get; init; }

    [RegularExpression(@"^\d{4}$", ErrorMessage = "PIN must be exactly 4 digits")]
    public required string PinCode { get; init; }
}