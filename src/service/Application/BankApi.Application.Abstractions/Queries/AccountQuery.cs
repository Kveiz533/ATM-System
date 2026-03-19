using BankApi.Domain.Accounts;
using SourceKit.Generators.Builder.Annotations;

namespace BankApi.Application.Abstractions.Queries;

[GenerateBuilder]
public sealed partial record AccountQuery(
    AccountId[] AccountIds,
    string[] AccountNumbers,
    long? KeyCursor,
    [RequiredValue] int PageSize);
