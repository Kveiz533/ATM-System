using BankApi.Cli.Application.Contracts.Users;
using BankApi.Cli.Application.Contracts.Users.Operations;
using BankApi.Cli.Presentation.Cli.Commands;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace BankApi.Cli.Presentation.Cli.CommandHandlers;

public sealed class WithdrawCommandHandler : AsyncCommand<WithdrawCommand>
{
    private readonly IUserService _service;

    public WithdrawCommandHandler(IUserService service)
    {
        _service = service;
    }

    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        WithdrawCommand settings,
        CancellationToken cancellationToken)
    {
        var serviceRequest = new Withdraw.Request(settings.Money);

        Withdraw.Response response = await _service.WithdrawAsync(
            serviceRequest,
            cancellationToken);

        if (response is Withdraw.Response.Success success)
        {
            AnsiConsole.MarkupLine($"[green]Withdraw successful![/] New balance: [yellow]{success.Balance}[/]");
            return 0;
        }

        if (response is Withdraw.Response.Failure failure)
        {
            AnsiConsole.MarkupLine($"[red]Error occured:[/] '{failure.Message}'");
            return 1;
        }

        throw new UnreachableException();
    }
}