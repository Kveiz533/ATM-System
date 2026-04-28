using System.ComponentModel.DataAnnotations;

namespace BankApi.Infrastructure.Persistence.Options;

public class PostgresOptions
{
    [MinLength(1)]
    public required string Host { get; init; } = string.Empty;

    [Range(1, 65535)]
    public required int Port { get; init; }

    public required string Database { get; init; } = string.Empty;

    public required string Username { get; init; } = string.Empty;

    public required string Password { get; init; } = string.Empty;

    public string ToConnectionString()
    {
        return $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password};";
    }
}
