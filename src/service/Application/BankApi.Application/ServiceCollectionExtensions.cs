using BankApi.Application.Contracts.Accounts;
using BankApi.Application.Contracts.Admins;
using BankApi.Application.Contracts.Sessions;
using BankApi.Application.Options;
using BankApi.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BankApi.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddOptions<AdminOptions>()
            .BindConfiguration("AdminSettings")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        collection.AddScoped<IAdminService, AdminService>();
        collection.AddScoped<ISessionService, SessionService>();
        collection.AddScoped<IAccountService, AccountService>();

        return collection;
    }
}
