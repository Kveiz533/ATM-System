using BankApi.Cli.Application.Contracts.Users;
using BankApi.Cli.Application.Contracts.Users.Operations;
using BankApi.Cli.Presentation.Cli.Commands;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace BankApi.Cli.Presentation.Cli.CommandHandlers;

public sealed class DepositCommandHandler : AsyncCommand<DepositCommand>
{
    private readonly IUserService _service;

    public DepositCommandHandler(IUserService service)
    {
        _service = service;
    }

    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        DepositCommand settings,
        CancellationToken cancellationToken)
    {
        var serviceRequest = new Deposit.Request(settings.Money);

        Deposit.Response response = await _service.DepositAsync(
            serviceRequest,
            cancellationToken);

        if (response is Deposit.Response.Success success)
        {
            AnsiConsole.MarkupLine($"[green]Deposit successful![/] New balance: [yellow]{success.Balance}[/]");
            return 0;
        }

        if (response is Deposit.Response.Failure failure)
        {
            AnsiConsole.MarkupLine($"[red]Error occured:[/] '{failure.Message}'");
            return 1;
        }

        throw new UnreachableException();
    }
}