using BankApi.Cli.Application.Contracts.Admins;
using BankApi.Cli.Application.Contracts.Admins.Operations;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace BankApi.Cli.Presentation.Cli.CommandHandlers;

public sealed class CreateAccountCommandHandler : AsyncCommand
{
    private readonly IAdminService _service;

    public CreateAccountCommandHandler(IAdminService service)
    {
        _service = service;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        string accountNumber = AnsiConsole.Ask<string>("Enter new [blue]account number[/] (20 digits):");

        string pinCode = AnsiConsole.Ask<string>("Enter [green]initial PIN[/] (4 digits):");

        var request = new CreateAccount.Request(accountNumber, pinCode);
        CreateAccount.Response response = await _service.CreateAccountAsync(request, cancellationToken);

        if (response is CreateAccount.Response.Success)
        {
            AnsiConsole.MarkupLine($"[green]Account {accountNumber} created successfully![/]");
            return 0;
        }

        if (response is CreateAccount.Response.Failure failure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create account:[/] {failure.Message}");
            return 1;
        }

        throw new UnreachableException();
    }
}