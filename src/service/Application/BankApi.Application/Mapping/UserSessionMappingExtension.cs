using BankApi.Application.Contracts.Sessions.Models;
using BankApi.Domain.Sessions;

namespace BankApi.Application.Mapping;

public static class UserSessionMappingExtension
{
    public static SessionDto MapToDto(this UserSession userSession)
    {
        return new SessionDto(userSession.SessionId);
    }
}
