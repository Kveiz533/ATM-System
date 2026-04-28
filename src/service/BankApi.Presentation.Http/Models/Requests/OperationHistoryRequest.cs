using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BankApi.Presentation.Http.Models.Requests;

public sealed class OperationHistoryRequest
{
    public required Guid SessionId { get; init; }

    [FromQuery(Name = "pageToken")]
    public string? PageToken { get; init; }

    [FromQuery(Name = "pageSize")]
    [Range(minimum: 1, maximum: 1000)]
    public int PageSize { get; init; }
}