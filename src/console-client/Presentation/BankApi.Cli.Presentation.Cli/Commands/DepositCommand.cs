using Spectre.Console.Cli;
using System.ComponentModel;

namespace BankApi.Cli.Presentation.Cli.Commands;

public sealed class DepositCommand : CommandSettings
{
    [CommandArgument(0, "<MONEY>")]
    [Description("Amount to deposit the account")]
    public required decimal Money { get; init; }
}