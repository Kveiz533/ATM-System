using BankApi.Cli.Application.Contracts.Admins;
using BankApi.Cli.Application.Contracts.Admins.Operations;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace BankApi.Cli.Presentation.Cli.CommandHandlers;

public sealed class LogInAdminCommandHandler : AsyncCommand
{
    private readonly IAdminService _service;

    public LogInAdminCommandHandler(IAdminService service)
    {
        _service = service;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        string password = AnsiConsole.Prompt(
            new TextPrompt<string>("Please enter [blue]admin password[/]:")
                .PromptStyle("grey")
                .Secret());

        var serviceRequest = new LogInAdmin.Request(password);

        LogInAdmin.Response response = await _service.LogInAdminAsync(
            serviceRequest,
            cancellationToken);

        if (response is LogInAdmin.Response.Success success)
        {
            AnsiConsole.MarkupLine("[green]Login Successful![/]");
            return 0;
        }

        if (response is LogInAdmin.Response.Failure failure)
        {
            AnsiConsole.MarkupLine($"[red]Error occured:[/] '{failure.Message}'");
            return 1;
        }

        throw new UnreachableException();
    }
}