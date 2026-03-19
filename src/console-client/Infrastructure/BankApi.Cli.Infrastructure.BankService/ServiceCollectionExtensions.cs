using BankApi.Cli.Application.Abstractions.Admins;
using BankApi.Cli.Application.Abstractions.Users;
using BankApi.Cli.Infrastructure.BankService.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;

namespace BankApi.Cli.Infrastructure.BankService;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBankClient(this IServiceCollection collection)
    {
        collection
            .AddOptions<BankClientOptions>()
            .BindConfiguration("Infrastructure:Configuration")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        collection
            .AddRefitClient<IRefitUserClient>()
            .ConfigureHttpClient((provider, client) =>
            {
                IOptions<BankClientOptions> options = provider.GetRequiredService<IOptions<BankClientOptions>>();
                client.BaseAddress = options.Value.Address;
            });

        collection
            .AddRefitClient<IRefitAdminClient>()
            .ConfigureHttpClient((provider, client) =>
            {
                IOptions<BankClientOptions> options = provider.GetRequiredService<IOptions<BankClientOptions>>();
                client.BaseAddress = options.Value.Address;
            });

        collection.AddScoped<IAdminClient, AdminClient>();
        collection.AddScoped<IUserClient, UserClient>();

        return collection;
    }
}