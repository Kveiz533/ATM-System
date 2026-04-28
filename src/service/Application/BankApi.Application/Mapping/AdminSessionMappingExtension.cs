using BankApi.Application.Contracts.Sessions.Models;
using BankApi.Domain.Sessions;

namespace BankApi.Application.Mapping;

public static class AdminSessionMappingExtension
{
    public static SessionDto MapToDto(this AdminSession adminSession)
    {
        return new SessionDto(adminSession.SessionId);
    }
}