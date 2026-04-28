using BankApi.Cli.Application.Contracts.Users;
using BankApi.Cli.Application.Contracts.Users.Models;
using BankApi.Cli.Application.Contracts.Users.Operations;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace BankApi.Cli.Presentation.Cli.CommandHandlers;

public sealed class OperationHistoryCommandHandler : AsyncCommand
{
    private readonly IUserService _service;

    public OperationHistoryCommandHandler(IUserService service)
    {
        _service = service;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        OperationHistory.Response response = await _service.OperationHistoryAsync(cancellationToken);

        if (response is OperationHistory.Response.Failure failure)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] {failure.Message}");
            return 1;
        }

        if (response is OperationHistory.Response.Success success)
        {
            if (success.History.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No operations found.[/]");
                return 0;
            }

            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.Title("[blue]Transaction History[/]");

            table.AddColumn("Time");
            table.AddColumn("Type");
            table.AddColumn(new TableColumn("Balance").RightAligned());

            foreach (OperationHistoryEntity operation in success.History)
            {
                string color = operation.BankOperationType switch
                {
                    var t when t.Contains("Withdraw", StringComparison.OrdinalIgnoreCase) => "red",
                    var t when t.Contains("Deposit", StringComparison.OrdinalIgnoreCase) => "green",
                    _ => "blue",
                };

                table.AddRow(
                    operation.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                    $"[{color}]{operation.BankOperationType}[/]",
                    $"[yellow]{operation.Balance:N2}[/]");
            }

            AnsiConsole.Write(table);
            return 0;
        }

        throw new UnreachableException();
    }
}