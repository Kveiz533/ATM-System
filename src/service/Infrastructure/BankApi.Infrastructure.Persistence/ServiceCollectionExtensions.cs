using BankApi.Application.Abstractions;
using BankApi.Application.Abstractions.Repositories;
using BankApi.Infrastructure.Persistence.Options;
using BankApi.Infrastructure.Persistence.Repositories;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankApi.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructurePersistence(
        this IServiceCollection collection,
        IConfiguration configuration)
    {
        IConfigurationSection postgresSection = configuration.GetSection("Infrastructure:Persistence:Postgres");

        collection.AddOptions<PostgresOptions>()
            .BindConfiguration("Infrastructure:Persistence:Postgres")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        PostgresOptions postgresOptions = postgresSection.Get<PostgresOptions>()
                                          ?? throw new InvalidOperationException("Postgres configuration not found");

        string connectionString = postgresOptions.ToConnectionString();

        collection.AddNpgsqlDataSource(connectionString);

        collection
            .AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .WithMigrationsIn(typeof(IAssemblyMarker).Assembly))
            .AddLogging(logging => logging.AddFluentMigratorConsole());

        collection.AddScoped<IPersistenceContext, PersistenceContext>();

        collection.AddScoped<IAccountRepository, AccountRepository>();
        collection.AddScoped<IAdminSessionRepository, AdminSessionRepository>();
        collection.AddScoped<IOperationHistoryRepository, OperationHistoryRepository>();
        collection.AddScoped<IUserSessionRepository, UserSessionRepository>();

        return collection;
    }
}