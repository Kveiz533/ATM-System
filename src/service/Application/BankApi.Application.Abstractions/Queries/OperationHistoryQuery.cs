using BankApi.Domain.Accounts;
using BankApi.Domain.BankOperations;
using SourceKit.Generators.Builder.Annotations;

namespace BankApi.Application.Abstractions.Queries;

[GenerateBuilder]
public sealed partial record OperationHistoryQuery(
    AccountId[] AccountIds,
    BankOperationId? KeyCursor,
    [RequiredValue] int PageSize);