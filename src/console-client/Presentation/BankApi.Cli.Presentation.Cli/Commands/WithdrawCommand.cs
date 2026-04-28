using Spectre.Console.Cli;
using System.ComponentModel;

namespace BankApi.Cli.Presentation.Cli.Commands;

public sealed class WithdrawCommand : CommandSettings
{
    [CommandArgument(0, "<MONEY>")]
    [Description("Amount to withdraw the account")]
    public required decimal Money { get; init; }
}