using System.CommandLine;
using TransactionRunner.UseCase;

var defaultRootFolder = "_task";

var inputBalanceArg = new Argument<FileInfo>("input_balance")
{
    Description = "The input balance file",
    DefaultValueFactory = _ => new FileInfo($"{defaultRootFolder}/account_balances.csv")
};

var inputTransactionsArg = new Argument<FileInfo>("input_transactions")
{
    Description = "The input transactions file",
    DefaultValueFactory = _ => new FileInfo($"{defaultRootFolder}/transactions.csv")
};

var outputBalanceArg = new Argument<FileInfo>("output_balance")
{
    Description = "The output balance file",
    DefaultValueFactory = _ => new FileInfo($"{defaultRootFolder}/output_balances.csv")
};

var declinedTransactionsArg = new Argument<FileInfo>("declined_transactions")
{
    Description = "The declined transactions file",
    DefaultValueFactory = _ => new FileInfo($"{defaultRootFolder}/declined_transactions.csv")
};

var processCommand = new Command("process", "Process daily transactions");
processCommand.Arguments.Add(inputBalanceArg);
processCommand.Arguments.Add(inputTransactionsArg);
processCommand.Arguments.Add(outputBalanceArg);
processCommand.Arguments.Add(declinedTransactionsArg);

processCommand.SetAction(parseResult =>
{
    var inputBalanceFile = parseResult.GetValue(inputBalanceArg);
    if (inputBalanceFile == null || !inputBalanceFile.Exists)
    {
        Console.WriteLine($"Input balance file '{inputBalanceFile?.FullName}' does not exist.");
        return 1;
    }
    var inputTransactionsFile = parseResult.GetValue(inputTransactionsArg);
    if (inputTransactionsFile == null || !inputTransactionsFile.Exists)
    {
        Console.WriteLine($"Input transactions file '{inputTransactionsFile?.FullName}' does not exist.");
        return 2;
    }
    var outputBalanceFile = parseResult.GetValue(outputBalanceArg);
    if (outputBalanceFile == null)
    {
        Console.WriteLine("Output balance file is not specified.");
        return 3;
    }
    if (outputBalanceFile.Exists)
    {
        outputBalanceFile.Delete();
    }

    var declinedTransactionsFile = parseResult.GetValue(declinedTransactionsArg);
    if (declinedTransactionsFile == null)
    {
        Console.WriteLine("Declined transactions file is not specified.");
        return 4;
    }
    if (declinedTransactionsFile.Exists)
    {
        declinedTransactionsFile.Delete();
    }

    var processor = new DailyTransactions();
    var result = processor.ProcessTransactions(inputBalanceFile, inputTransactionsFile, outputBalanceFile, declinedTransactionsFile);
    return result ? 0 : 5;
});

var rootCommand = new RootCommand();

rootCommand.Subcommands.Add(processCommand);

return rootCommand.Parse(args).Invoke();

