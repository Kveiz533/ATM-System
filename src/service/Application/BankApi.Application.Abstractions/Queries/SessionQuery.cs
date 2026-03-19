using SourceKit.Generators.Builder.Annotations;

namespace BankApi.Application.Abstractions.Queries;

[GenerateBuilder]
public sealed partial record SessionQuery(
    Guid[] SessionIds,
    Guid? KeyCursor,
    [RequiredValue] int PageSize);