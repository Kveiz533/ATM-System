using BankApi.Cli.Application;
using BankApi.Cli.Application.Abstractions.SessionProviders;
using BankApi.Cli.Application.Providers;
using BankApi.Cli.Infrastructure.BankService;
using BankApi.Cli.Presentation.Cli.CommandHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;

IConfigurationRoot configuration = new ConfigurationManager()
    .AddJsonFile("appsettings.json", optional: false)
    .AddUserSecrets<Program>()
    .Build();

var sessionProvider = new SessionProvider();

IServiceCollection collection = new ServiceCollection()
    .AddSingleton<ISessionProvider>(sessionProvider)
    .AddApplication()
    .AddBankClient()
    .AddSingleton<IConfiguration>(configuration);

var registrar = new ServiceCollectionRegistrar(collection);
var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.SetApplicationName("bank-cli");

    config.AddCommand<LogInUserCommandHandler>("login")
        .WithDescription("Login to your bank account");

    config.AddCommand<LogInAdminCommandHandler>("admin-login")
        .WithDescription("Login as system administrator");

    config.AddCommand<BalanceCommandHandler>("balance")
        .WithDescription("Show current balance");

    config.AddCommand<DepositCommandHandler>("deposit")
        .WithDescription("Deposit money to your account");

    config.AddCommand<WithdrawCommandHandler>("withdraw")
        .WithDescription("Withdraw money from your account");

    config.AddCommand<OperationHistoryCommandHandler>("history")
        .WithDescription("Show transaction history");

    config.AddCommand<CreateAccountCommandHandler>("create-account")
        .WithDescription("Create a new bank account");
});

AnsiConsole.Write(new FigletText("Bank_Api").Color(Color.Blue));
AnsiConsole.MarkupLine("[grey]Type [white]'exit'[/] to quit, [white]'--help'[/] for commands.[/]");

while (true)
{
    string input = AnsiConsole.Ask<string>("[bold blue]bank-cli>[/]");

    if (string.IsNullOrWhiteSpace(input)) continue;

    if (input.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    string[] newArgs = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    await app.RunAsync(newArgs);
    AnsiConsole.WriteLine();
}

return 0;