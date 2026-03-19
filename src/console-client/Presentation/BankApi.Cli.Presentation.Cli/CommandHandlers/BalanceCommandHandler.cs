using BankApi.Cli.Application.Contracts.Users;
using BankApi.Cli.Application.Contracts.Users.Operations;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace BankApi.Cli.Presentation.Cli.CommandHandlers;

public sealed class BalanceCommandHandler : AsyncCommand
{
    private readonly IUserService _service;

    public BalanceCommandHandler(IUserService service)
    {
        _service = service;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        Balance.Response response = await _service.BalanceAsync(cancellationToken);

        if (response is Balance.Response.Success success)
        {
            AnsiConsole.MarkupLine($"[green]Current balance[/]: [yellow]{success.Balance}[/]");
            return 0;
        }

        if (response is Balance.Response.Failure failure)
        {
            AnsiConsole.MarkupLine($"[red]Error occured:[/] '{failure.Message}'");
            return 1;
        }

        throw new UnreachableException();
    }
}