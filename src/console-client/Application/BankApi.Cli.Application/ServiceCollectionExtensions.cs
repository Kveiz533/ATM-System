using BankApi.Cli.Application.Contracts.Admins;
using BankApi.Cli.Application.Contracts.Users;
using BankApi.Cli.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BankApi.Cli.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddScoped<IUserService, UserService>();
        collection.AddScoped<IAdminService, AdminService>();

        return collection;
    }
}