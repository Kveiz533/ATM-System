using BankApi.Application.Abstractions;
using BankApi.Application.Abstractions.Repositories;

namespace BankApi.Infrastructure.Persistence;

public class PersistenceContext : IPersistenceContext
{
    public PersistenceContext(
        IAccountRepository accountRepository,
        IUserSessionRepository userSessionRepository,
        IAdminSessionRepository adminSessionRepository,
        IOperationHistoryRepository operationHistoryRepository)
    {
        AccountRepository = accountRepository;
        UserSessionRepository = userSessionRepository;
        AdminSessionRepository = adminSessionRepository;
        OperationHistoryRepository = operationHistoryRepository;
    }

    public IAccountRepository AccountRepository { get; }

    public IUserSessionRepository UserSessionRepository { get; }

    public IAdminSessionRepository AdminSessionRepository { get; }

    public IOperationHistoryRepository OperationHistoryRepository { get; }
}