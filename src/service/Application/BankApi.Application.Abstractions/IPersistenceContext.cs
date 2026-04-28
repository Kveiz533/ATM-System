using BankApi.Application.Abstractions.Repositories;

namespace BankApi.Application.Abstractions;

public interface IPersistenceContext
{
    IAccountRepository AccountRepository { get; }

    IUserSessionRepository UserSessionRepository { get; }

    IAdminSessionRepository AdminSessionRepository { get; }

    IOperationHistoryRepository OperationHistoryRepository { get; }
}