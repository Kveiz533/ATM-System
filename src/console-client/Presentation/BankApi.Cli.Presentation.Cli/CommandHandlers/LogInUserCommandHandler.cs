using BankApi.Cli.Application.Contracts.Users;
using BankApi.Cli.Application.Contracts.Users.Operations;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace BankApi.Cli.Presentation.Cli.CommandHandlers;

public sealed class LogInUserCommandHandler : AsyncCommand
{
    private readonly IUserService _service;

    public LogInUserCommandHandler(IUserService service)
    {
        _service = service;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        string accountNumber = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter your [blue]account number[/]:")
                .Validate(acc => acc.Length == 20
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[yellow]Account number must be 20 digits[/]")));

        string pin = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter your [green]PIN[/]:")
                .PromptStyle("grey")
                .Secret()
                .Validate(p => p.Length == 4
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[yellow]PIN must be 4 digits[/]")));

        var request = new LogInUser.Request(accountNumber, pin);
        LogInUser.Response response = await _service.LogInUserAsync(request, cancellationToken);

        if (response is LogInUser.Response.Success success)
        {
            AnsiConsole.MarkupLine("[green]Successfully logged in![/]");
            return 0;
        }

        if (response is LogInUser.Response.Failure failure)
        {
            AnsiConsole.MarkupLine($"[red]Login failed:[/] {failure.Message}");
            return 1;
        }

        throw new UnreachableException();
    }
}